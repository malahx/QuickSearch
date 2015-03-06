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
using UnityEngine;

namespace QuickSearch {
	internal class QGUI {

		private static string BackTexturePath = Quick.MOD + "/Textures/back";
		private static string SaveTexturePath = Quick.MOD + "/Textures/bookmark";
		private static string DeleteTexturePath = Quick.MOD + "/Textures/delete";
		internal static Texture2D BackTexture;
		internal static Texture2D SaveTexture;
		internal static Texture2D DeleteTexture;
		private static Rect RectPartsListSimple;
		private static Rect RectPartsListAdvanced;
		private static Rect RectOthersSimple;
		private static Rect RectOthersAdvanced;
		private static GUIStyle TextField;

		internal static bool Ready = false;

		internal static void Init() {
			BackTexture = GameDatabase.Instance.GetTexture (BackTexturePath, false);
			SaveTexture = GameDatabase.Instance.GetTexture (SaveTexturePath, false);
			DeleteTexture = GameDatabase.Instance.GetTexture (DeleteTexturePath, false);
			RectPartsListSimple = new Rect(55, Screen.height - 135, 180, 30);
			RectPartsListAdvanced = new Rect(90, Screen.height - 135, 180, 30);
			RectOthersSimple = new Rect(85, Screen.height - 105, 115, 40);
			RectOthersAdvanced = new Rect(120, Screen.height - 105, 115, 40);
			TextField = HighLogic.Skin.textField;
			TextField.stretchWidth = true;
			TextField.fixedHeight = 20;
			TextField.alignment = TextAnchor.MiddleCenter;
			Ready = true;
			Quick.Log ("GUI Init");
		}

		internal static void OnGUI() {
			GUI.skin = HighLogic.Skin;
			PartCategorizer.Category _currentFilter = QCategory.CurrentFilter;
			PartCategorizer.Category _currentSubCategory = QCategory.CurrentSubCategory;
			if (_currentFilter != null) {
				if (_currentFilter == QCategory.FilterPartSearch || _currentFilter.displayType == EditorPartList.State.SubassemblyList) {
					if (_currentSubCategory == null) {
						QCategory.Refresh ();
					} else {
						if (_currentSubCategory != QCategory.SubCategoryPartSearch && _currentSubCategory != _currentFilter.subcategories [0]) {
							if (QSearch.Text != _currentSubCategory.button.categoryName) {
								QSearch.Text = _currentSubCategory.button.categoryName;
								QCategory.Refresh ();
							}
						}
					}
				} else {
					QSearch.Text = string.Empty;
				}
				Rect _rectSimple = (_currentFilter.displayType == EditorPartList.State.PartsList ? RectPartsListSimple : RectOthersSimple);
				Rect _rectAdvanced = (_currentFilter.displayType == EditorPartList.State.PartsList ? RectPartsListAdvanced : RectOthersAdvanced);

				DrawSearch ((EditorLogic.Mode == EditorLogic.EditorModes.SIMPLE ? _rectSimple : _rectAdvanced));

				if (_currentFilter == QCategory.FilterPartSearch) {
					if (_currentFilter.displayType == EditorPartList.State.PartsList) {
						DrawButton ((EditorLogic.Mode == EditorLogic.EditorModes.SIMPLE ? RectOthersSimple : RectOthersAdvanced));
					}
				}
			}
		}

		private static void DrawSearch(Rect rectArea) {
			string _Text = QSearch.CleanInput (QSearch.Text);
			if (_Text != QSearch.Text) {
				QSearch.Text = _Text;
			}
			GUILayout.BeginArea (rectArea);
			DrawSpace ();
			GUILayout.BeginVertical ();
			_Text = GUILayout.TextField (QSearch.Text, TextField);
			if (_Text != QSearch.Text) {
				if (PartListTooltips.fetch.displayTooltip) {
					GameEvents.onTooltipDestroyRequested.Fire();
					PartListTooltips.fetch.HideTooltip ();
				}
				QSearch.Text = _Text;
				QCategory.Refresh ();
			}
			GUILayout.EndVertical ();
			DrawSpace ();
			GUILayout.EndArea ();
		}

		private static void DrawButton(Rect rectArea) {
			GUILayout.BeginArea (rectArea);
			DrawSpace ();
			GUILayout.BeginVertical ();
			GUILayout.BeginHorizontal ();
			if (QCategory.lastFilter != null || QCategory.lastSubCategory != null) {
				if (GUILayout.Button (new GUIContent (BackTexture, "Back to the last Category Selected"), GUILayout.Width (30), GUILayout.Height (30))) {
					QCategory.GoToLastCategory ();
				}
			}
			GUILayout.FlexibleSpace ();
			if (QSearch.Text != string.Empty) {
				if (GUILayout.Button (new GUIContent ((QCategory.Exists (QSearch.Text) ? DeleteTexture : SaveTexture), "Bookmark"), GUILayout.Width (30), GUILayout.Height (30))) {
					QCategory.Bookmark (QSearch.Text);
				}
			}
			GUILayout.EndHorizontal ();
			GUILayout.EndVertical ();
			DrawSpace ();
			GUILayout.EndArea ();
		}

		private static void DrawSpace() {
			GUILayout.BeginVertical ();
			GUILayout.Space (5);
			GUILayout.EndVertical ();
		}
	}
}