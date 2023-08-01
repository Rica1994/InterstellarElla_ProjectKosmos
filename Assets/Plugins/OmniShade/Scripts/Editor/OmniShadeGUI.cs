//------------------------------------
//             OmniShade
//     Copyright© 2023 OmniShade     
//------------------------------------

using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

/**
 * This class manages the GUI for the shader, automatically enabling/disabling shader keywords
 * to generate the appropriate shader variants at build time.
 **/
public static class OmniShade {
	public const string NAME = "OmniShade";
	public const string DOCS_URL = "https://www.omnishade.io/documentation/features";
	public const string PRO_URL = "https://assetstore.unity.com/packages/vfx/shaders/omnishade-mobile-optimized-shader-215111";
}

public class OmniShadeGUI : ShaderGUI {
	// Shader keywords which are automatically enabled/disabled based on usage
	readonly List<(string keyword, string name, PropertyType type, Vector4 defaultValue)> props = new List<(string keyword, string name, PropertyType type, Vector4 defaultValue)>() {
		("BASE_CONTRAST", "_Contrast", PropertyType.Float, Vector4.one),
		("BASE_SATURATION", "_Saturation", PropertyType.Float, Vector4.one),
		("MATCAP", "_MatCapTex", PropertyType.Texture, Vector4.one),
		("MATCAP_CONTRAST", "_MatCapContrast", PropertyType.Float, Vector4.one),
		("NORMAL_MAP", "_NormalTex", PropertyType.Texture, Vector4.one),
		("AMBIENT", "_AmbientBrightness", PropertyType.Float, Vector4.zero),
		("ZOFFSET", "_ZOffset", PropertyType.Float, Vector4.zero),
	};

	// Parameters that are ON by default
	readonly List<(string keyword, string name)> defaultOnParams = new List<(string keyword, string name)>() {
		("MATCAP_PERSPECTIVE", "_MatCapPerspective" ),
		("SHADOWS_ENABLED", "_ShadowsEnabled" ),
		("FOG", "_Fog" ),
	};

	enum PropertyType {
		Float, Vector, Texture
	};

	struct PropertyHeader {
		public string headerName;
		public bool isOpen;
		public PropertyHeader(string _header, bool _isOpen) {
			this.headerName = _header;
			this.isOpen = _isOpen;
		}
	};

	const string HEADER_GROUP = "HeaderGroup";

	enum ExpandType {
		All,
		Active,
		Collapse,
		Keep,
	};

	ExpandType forceExpand = ExpandType.Active;
	int prevPreset = -1;
	List<Material> prevSelectedMats = new List<Material>();
	readonly Dictionary<string, PropertyHeader> propertyHeaders = new Dictionary<string, PropertyHeader>();

	static readonly Dictionary<string, GUIContent> toolTipsCache = new Dictionary<string, GUIContent>();

	public override void OnGUI(MaterialEditor materialEditor, MaterialProperty[] properties) {
		this.RenderGUI(materialEditor, properties);

		// Multi-selection
		var mat = materialEditor.target as Material;
		var mats = new List<Material>();
		if (mat != null)
			mats.Add(mat);
		foreach (var selected in Selection.objects) {
			if (selected.GetType() == typeof(Material)) {
				var selectedMat = selected as Material;
				if (selectedMat != mat && selectedMat != null &&
					selectedMat.shader.name.Contains(OmniShade.NAME))
					mats.Add(selectedMat);
			}
		}

		// Loop selected materials
		foreach (var material in mats) {
			this.AutoEnableShaderKeywords(material);
			this.UpdatePresetValues(material);

			// Reset previous preset if multi-selection
			if (mats.Count > 1)
				this.prevPreset = -1;
		}

		// Reset previous preset if selection changed
		if (mats.Count == 1 && this.prevSelectedMats.Count == 1 &&
			mats[0] != null && this.prevSelectedMats[0] != null && mats[0].name != this.prevSelectedMats[0].name)
			this.prevPreset = -1;
		this.prevSelectedMats = mats;
	}

	void AutoEnableShaderKeywords(Material mat) {
		foreach (var prop in this.props) {
			if (!mat.HasProperty(prop.name))
				continue;

			// Check if property value is being used (not set to default)
			bool isInUse = false;
			switch (prop.type) {
				case PropertyType.Float:
					isInUse = mat.GetFloat(prop.name) != prop.defaultValue.x;
					break;
				case PropertyType.Vector:
					isInUse = mat.GetVector(prop.name) != prop.defaultValue;
					break;
				case PropertyType.Texture:
					isInUse = mat.GetTexture(prop.name) != null;
					break;
				default:
					break;
			}

			// Enable or disable shader keyword
			if (isInUse) {
				if (!mat.IsKeywordEnabled(prop.keyword))
					mat.EnableKeyword(prop.keyword);
			}
			else if (mat.IsKeywordEnabled(prop.keyword))
				mat.DisableKeyword(prop.keyword);
		}

		// Set keywords for parameters that are ON by default
		foreach (var defaultOnParam in this.defaultOnParams) {
			if (mat.HasProperty(defaultOnParam.name) && mat.GetFloat(defaultOnParam.name) == 1)
				mat.EnableKeyword(defaultOnParam.keyword);
		}

		this.AutoConfigureProperties(mat);
	}

	void AutoConfigureProperties(Material mat) {
		// MatCap Static Rotation default angle points to first camera found
		if (mat.IsKeywordEnabled("MATCAP_STATIC") && mat.HasProperty("_MatCapRot") &&
			mat.GetVector("_MatCapRot") == Vector4.zero) {
			var cam = GameObject.FindObjectOfType<Camera>();
			if (cam != null) {
				var matCapRot = -cam.transform.rotation.eulerAngles * Mathf.PI / 180;
				mat.SetVector("_MatCapRot", matCapRot);
			}
		}
	}

	void UpdatePresetValues(Material mat) {
		int preset = (int)mat.GetFloat("_Preset");

		// Do nothing if preset was not changed
		if (this.prevPreset == -1 || this.prevPreset == preset) {
			this.prevPreset = preset;
			return;
		}

		mat.SetFloat("_Cull", 2);                   // Back
		mat.SetFloat("_ZTest", 4);                  // LessEqual
		mat.SetFloat("_BlendOp", 0);                // Add
		switch (preset) {
			case 0:                                 // OPAQUE
				mat.SetFloat("_ZWrite", 1);
				mat.SetFloat("_SourceBlend", 1);    // One
				mat.SetFloat("_DestBlend", 0);      // Zero
				if (mat.renderQueue >= 2450)
					mat.renderQueue = 2000;
				mat.SetFloat("_Cutout", 0);         // Cutout
				mat.DisableKeyword("CUTOUT");
				break;

			case 1:                                 // TRANSPARENT
				mat.SetFloat("_ZWrite", 0);
				mat.SetFloat("_SourceBlend", 5);    // SrcAlpha
				mat.SetFloat("_DestBlend", 10);     // OneMinusSrcAlpha
				if (mat.renderQueue < 3000)
					mat.renderQueue = 3000;
				mat.SetFloat("_Cutout", 0);         // Cutout
				mat.DisableKeyword("CUTOUT");
				break;

			case 2:                                 // TRANSPARENT ADDITIVE
				mat.SetFloat("_ZWrite", 0);
				mat.SetFloat("_SourceBlend", 1);    // One
				mat.SetFloat("_DestBlend", 1);      // One
				if (mat.renderQueue < 3000)
					mat.renderQueue = 3000;
				mat.SetFloat("_Cutout", 0);         // Cutout
				mat.DisableKeyword("CUTOUT");
				break;

			case 3:                                 // TRANSPARENT ADDITIVE ALPHA
				mat.SetFloat("_ZWrite", 0);
				mat.SetFloat("_SourceBlend", 5);    // SrcAlpha
				mat.SetFloat("_DestBlend", 1);      // One
				if (mat.renderQueue < 3000)
					mat.renderQueue = 3000;
				mat.SetFloat("_Cutout", 0);         // Cutout
				mat.DisableKeyword("CUTOUT");
				break;

			case 4:                                 // OPAQUE CUTOUT
				mat.SetFloat("_Cull", 0);           // Disabled
				mat.SetFloat("_ZWrite", 1);
				mat.SetFloat("_SourceBlend", 1);    // One
				mat.SetFloat("_DestBlend", 0);      // Zero
				mat.SetFloat("_Cutout", 1);         // Cutout
				mat.EnableKeyword("CUTOUT");
				if (mat.renderQueue < 2450 || mat.renderQueue >= 3000)
					mat.renderQueue = 2450;
				break;

			default:
				Debug.LogError(OmniShade.NAME + ": Unrecognized Preset (" + preset + ")");
				break;
		}

		this.prevPreset = preset;
	}

	void RenderGUI(MaterialEditor materialEditor, MaterialProperty[] properties) {
		materialEditor.SetDefaultGUIWidths();

		// Documentation button
		var content = new GUIContent(EditorGUIUtility.IconContent("_Help")) {
			text = OmniShade.NAME + " Docs",
			tooltip = OmniShade.DOCS_URL
		};
		if (GUILayout.Button(content))
			Help.BrowseURL(OmniShade.DOCS_URL);

		// Expand/Close all buttons
		GUILayout.BeginHorizontal();
		var expandAll = new GUIContent(EditorGUIUtility.IconContent("Toolbar Plus")) { text = "Expand All" };
		if (GUILayout.Button(expandAll))
			this.forceExpand = ExpandType.All;
		if (GUILayout.Button("Expand Active"))
			this.forceExpand = ExpandType.Active;
		var collapse = new GUIContent(EditorGUIUtility.IconContent("Toolbar Minus")) { text = "Collapse" };
		if (GUILayout.Button(collapse))
			this.forceExpand = ExpandType.Collapse;
		GUILayout.EndHorizontal();

		bool isLastFoldoutOpen = this.RenderShaderProperties(materialEditor, properties);

		// Append Unity rendering options to end of groups
		if (isLastFoldoutOpen) {
			materialEditor.RenderQueueField();
			EditorGUILayout.Separator();
			EditorGUILayout.LabelField("Rendering", EditorStyles.boldLabel);
			materialEditor.EnableInstancingField();
			materialEditor.DoubleSidedGIField();
		}
		EditorGUILayout.EndFoldoutHeaderGroup();

		// Pro Ad
		GUILayout.Space(10);
		if (GUILayout.Button("Upgrade to " + OmniShade.NAME))
			Help.BrowseURL(OmniShade.PRO_URL);
	}

	bool RenderShaderProperties(MaterialEditor materialEditor, MaterialProperty[] properties) {
		this.RefreshPropertyHeaders(materialEditor);

		bool isFoldoutOpen = true;
		string currentHeaderName = string.Empty;
		foreach (var prop in properties) {
			// If start of header, begin a new foldout group
			if (this.propertyHeaders.ContainsKey(prop.name)) {
				// Close previous foldout group
				if (!string.IsNullOrEmpty(currentHeaderName))
					EditorGUILayout.EndFoldoutHeaderGroup();

				// Begin foldout header
				var header = this.propertyHeaders[prop.name];
				currentHeaderName = header.headerName;
				var defaultColor = GUI.backgroundColor;
				isFoldoutOpen = header.isOpen = this.BeginFoldoutHeader(header.isOpen, header.headerName);
				this.propertyHeaders[prop.name] = header;
			}

			// Render shader property
			if (isFoldoutOpen)
				this.RenderShaderProperty(materialEditor, prop);
		}

		return isFoldoutOpen;
	}

	void RenderShaderProperty(MaterialEditor materialEditor, MaterialProperty prop) {
		string label = prop.displayName;
		var content = this.GetTooltip(label);
		materialEditor.ShaderProperty(prop, content);
	}

	void RefreshPropertyHeaders(MaterialEditor materialEditor) {
		var mat = materialEditor.target as Material;
		var shader = mat.shader;
		int numProps = shader.GetPropertyCount();

		var headerActiveDic = GetActivePropertyHeaders(mat);

		// Update property headers and check for new headers
		var newProps = new List<string>();
		for (int i = 0; i < numProps; i++) {
			var propAttrs = shader.GetPropertyAttributes(i);
			for (int j = 0; j < propAttrs.Length; j++) {
				// Skip if not a header attribute
				string propAttr = propAttrs[j];
				if (!this.IsHeaderGroup(propAttr))
					continue;

				string propName = shader.GetPropertyName(i);
				string headerName = this.GetHeaderGroupName(propAttr);
				newProps.Add(propName);

				// Update cache if something changed
				if (!this.propertyHeaders.ContainsKey(propName) ||
					this.propertyHeaders[propName].headerName != headerName || this.forceExpand != ExpandType.Keep) {
					bool isOpen = this.forceExpand == ExpandType.All;
					if (!this.propertyHeaders.ContainsKey(propName)) {          // New entry
						if (this.forceExpand == ExpandType.Keep || this.forceExpand == ExpandType.Active)
							isOpen = !headerActiveDic.ContainsKey(headerName) || headerActiveDic[headerName];
						var header = new PropertyHeader(headerName, isOpen);
						this.propertyHeaders.Add(propName, header);
					}
					else {                                                      // Update existing entry
						if (this.forceExpand == ExpandType.Keep)
							isOpen = this.propertyHeaders[propName].isOpen;
						else if (this.forceExpand == ExpandType.Active)
							isOpen = !headerActiveDic.ContainsKey(headerName) || headerActiveDic[headerName];
						var header = new PropertyHeader(headerName, isOpen);
						this.propertyHeaders[propName] = header;
					}
				}
			}
		}
		this.forceExpand = ExpandType.Keep;

		// Remove any headers that were deleted
		var propNames = new List<string>();
		foreach (var propName in this.propertyHeaders.Keys)
			propNames.Add(propName);
		var deletedProps = propNames.Except(newProps);
		foreach (var deletedProp in deletedProps)
			this.propertyHeaders.Remove(deletedProp);
	}

	Dictionary<string, bool> GetActivePropertyHeaders(Material mat) {
		var defaultOnHeaders = new string[] {
			"Culling And Blending"
		};

		var shader = mat.shader;
		int numProps = shader.GetPropertyCount();
		var headerActiveDic = new Dictionary<string, bool>();
		if (this.forceExpand == ExpandType.Active) {
			string currentHeaderName = string.Empty;
			for (int i = 0; i < numProps; i++) {
				// Check if header group
				var propAttrs = shader.GetPropertyAttributes(i);
				string headerName = this.GetHeaderGroupName(propAttrs);
				if (!string.IsNullOrEmpty(headerName))
					currentHeaderName = headerName;

				// Skip if no headers found yet
				if (string.IsNullOrEmpty(currentHeaderName))
					continue;

				// Check active headers
				bool isInUse = defaultOnHeaders.Contains(currentHeaderName) || this.IsPropertyActive(mat, i);
				if (headerActiveDic.ContainsKey(currentHeaderName))
					headerActiveDic[currentHeaderName] |= isInUse;
				else
					headerActiveDic.Add(currentHeaderName, isInUse);
			}
		}

		return headerActiveDic;
	}

	bool IsPropertyActive(Material mat, int propertyIndex) {
		var shader = mat.shader;
		string propName = shader.GetPropertyName(propertyIndex);
		string propDesc = shader.GetPropertyDescription(propertyIndex);
		var propType = shader.GetPropertyType(propertyIndex);

		if (!mat.HasProperty(propName))
			return false;
		if ((propType == ShaderPropertyType.Float && propDesc.StartsWith("Enable") && mat.GetFloat(propName) == 1) ||
			(propType == ShaderPropertyType.Texture && mat.GetTexture(propName) != null) ||
			(propName == "_AmbientBrightness" && mat.GetFloat("_AmbientBrightness") != 0))
			return true;
		return false;
	}

	bool IsHeaderGroup(string propAttr) {
		return propAttr.StartsWith(HEADER_GROUP);
	}

	string GetHeaderGroupName(string[] propAttrs) {
		for (int j = 0; j < propAttrs.Length; j++) {
			string propAttr = propAttrs[j];
			if (this.IsHeaderGroup(propAttr)) {
				return GetHeaderGroupName(propAttr);
			}
		}
		return null;
	}

	string GetHeaderGroupName(string propAttr) {
		int headerGroupLen = HEADER_GROUP.Length + 1;
		return propAttr.Substring(headerGroupLen, propAttr.LastIndexOf(")") - headerGroupLen);
	}

	bool BeginFoldoutHeader(bool isOpen, string label) {
		var content = this.GetTooltip(label);
		var defaultColor = GUI.backgroundColor;
		GUI.backgroundColor = new Color(1.35f, 1.35f, 1.35f);
		isOpen = EditorGUILayout.BeginFoldoutHeaderGroup(isOpen, content);
		GUI.backgroundColor = defaultColor;
		return isOpen;
	}

	GUIContent GetTooltip(string label) {
		// Check cache first
		if (OmniShadeGUI.toolTipsCache.ContainsKey(label))
			return OmniShadeGUI.toolTipsCache[label];

		string tooltip;
		switch (label) {
			case "Ignore Main Texture Alpha": tooltip = "Ignore the alpha channel on the texture, forcing it to be opaque."; break;
			case "Perspective Correction": tooltip = "Improves accuracy for dynamic cameras. For stationary cameras, disable to improve performance."; break;
			case "Use Static Rotation": tooltip = "Lock the MatCap rotation to a static angle."; break;
			case "Ambient Brightness": tooltip = "Intensity of the Environment Lighting from the Lighting Settings."; break;
			case "Culling And Blend Preset": tooltip = "Set cull and blend settings from presets."; break;
			case "Culling": tooltip = "Side of the geometry that is rendered."; break;
			case "Z Write": tooltip = "If enabled, this object occludes those behind it."; break;
			case "Z Test": tooltip = "Set to Always if this object should always render, even if behind others."; break;
			case "Depth Offset": tooltip = "Adjust the distance from the camera to tune visibility."; break;
			case "Cutout Transparency": tooltip = "Discards pixels with alpha less than 0.5."; break;
			default: tooltip = ""; break;
		}

		// Create tool tip and cache
		var content = new GUIContent()
		{
			text = label,
			tooltip = tooltip
		};
		OmniShadeGUI.toolTipsCache.Add(label, content);
		return content;
	}
}
