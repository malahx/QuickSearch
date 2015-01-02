/* 
QuickSearch
Copyright 2015 Malah

This program is free software: you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with this program.  If not, see <http://www.gnu.org/licenses/>. 
*/

using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;

namespace QuickSearch {
	[KSPAddon(KSPAddon.Startup.EditorAny, false)]
	public class QuickSearch : MonoBehaviour {

		public static string VERSION = "0.10";
		public static string MOD = "QuickSearch";

		private static bool isdebug = true;

		private Rect RectSimple;
		private Rect RectAdvanced;
		private GUIStyle SearchBox;
		private static Texture TextureIcon = (Texture)GameDatabase.Instance.GetTexture ("QuickSearch/Textures/icon", false);
		private static Texture TextureIconSelected = (Texture)GameDatabase.Instance.GetTexture ("QuickSearch/Textures/iconSelected", false);
		private PartCategorizer.Icon SearchIcon = new PartCategorizer.Icon ("NewSearch", TextureIcon, TextureIconSelected, false);
		private EditorPartListFilter<AvailablePart> SearchFilter = new EditorPartListFilter<AvailablePart> ("QuickSearch", part => Search (part));
		private PartCategorizer.Category SearchCat;
		private PartCategorizer.Category SearchSubCat;
		private static string SearchText = string.Empty;

		// Démarrage du plugin
		private void Start() {
			RectSimple = new Rect(85, Screen.height - 100, 115, 30);
			RectAdvanced = new Rect(120, Screen.height - 100, 115, 30);
			SearchBox = HighLogic.Skin.textField;
			SearchBox.stretchWidth = true;
			SearchBox.fixedHeight = 20;
			SearchBox.alignment = TextAnchor.MiddleCenter;
			SearchCat = new PartCategorizer.Category (PartCategorizer.ButtonType.FILTER, EditorPartList.State.PartsList, "QuickSearch", SearchIcon, new Color ((float)0.88, (float)0.53, (float)0.53), new Color(1, 1, 1), SearchFilter, true);
			SearchSubCat = new PartCategorizer.Category (PartCategorizer.ButtonType.SUBCATEGORY, EditorPartList.State.PartsList, "New Search", SearchIcon, new Color ((float)0.88, (float)0.53, (float)0.53), new Color(1, 1, 1), SearchFilter, true);
			SearchCat.AddSubcategory (SearchSubCat);
		}

		// Rechercher si la part correspond au patern
		private static bool Search(AvailablePart part) {
			if (part.category == PartCategories.none || part.TechRequired == "unassigned") {
				return false;
			}
			string _partinfo = (part.title + " " + part.author + " " + part.manufacturer + " " + part.name + " " + part.partSize + " " + part.resourceInfo + " " + part.TechRequired + " " + part.partPrefab + " " + part.moduleInfo + " " + part.description).ToLower ();
			string[] _SearchTexts = SearchText.ToLower().Split(' ');
			foreach (string _SearchText in _SearchTexts) {
				if (_partinfo.Contains (_SearchText)) {
					return true;
				}
			}
			return false;
		}

		// Interface
		public void OnGUI() {
			if (HighLogic.LoadedSceneIsEditor) {
				if (EditorLogic.fetch.editorScreen == EditorScreen.Parts && PartCategorizer.Ready) {
					if (EditorLogic.Mode == EditorLogic.EditorModes.SIMPLE) {
						GUILayout.BeginArea (RectSimple);
					} else {
						GUILayout.BeginArea (RectAdvanced);
					}
					GUILayout.BeginVertical ();
					GUILayout.Space (5);
					GUILayout.EndVertical ();
					GUILayout.BeginVertical ();
					string _SearchText = GUILayout.TextField (SearchText, SearchBox);
					if (_SearchText != SearchText) {
						if (PartListTooltips.fetch.displayTooltip) {
							PartListTooltips.fetch.HideTooltip ();
						}
						if (_SearchText != string.Empty) {
							PartCategorizer.Instance.SetAdvancedMode ();
							if (SearchCat.button.activeButton.State != RUIToggleButtonTyped.ButtonState.FALSE) {
								SearchCat.button.activeButton.SetFalse (SearchCat.button.activeButton, RUIToggleButtonTyped.ClickType.FORCED);
							}
							if (SearchCat.button.activeButton.State != RUIToggleButtonTyped.ButtonState.TRUE) {
								SearchCat.button.activeButton.SetTrue (SearchCat.button.activeButton, RUIToggleButtonTyped.ClickType.FORCED, false);
							}
							if (SearchSubCat.button.activeButton.State != RUIToggleButtonTyped.ButtonState.TRUE) {
								SearchSubCat.button.activeButton.SetTrue (SearchSubCat.button.activeButton, RUIToggleButtonTyped.ClickType.FORCED, false);
							}
							EditorPartList.Instance.Refresh ();
						}
						SearchText = _SearchText;
					}
					GUILayout.EndVertical ();
					GUILayout.BeginVertical ();
					GUILayout.Space (5);
					GUILayout.EndVertical ();
					GUILayout.EndArea ();
				}
			}
		}

		// Afficher les messages sur la console
		private static void Log(string String) {
			if (isdebug) {
				Debug.Log (MOD + "(" + VERSION + "): " + String);
			}
		}
	}
}