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
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;

namespace QuickSearch {
	internal class QGUI {
		#if !TINY
		private static string BackTexturePath = QuickSearch.MOD + "/Textures/back";
		internal static Texture2D BackTexture;

		private static string SaveTexturePath = QuickSearch.MOD + "/Textures/bookmark";
		internal static Texture2D SaveTexture;

		private static string DeleteTexturePath = QuickSearch.MOD + "/Textures/delete";
		internal static Texture2D DeleteTexture;
		#endif
		private static GUIStyle TextField;

		internal static bool Ready = false;

		private static Rect RectPartsList {
			get {
				#if TINY
				return RectOthers;
				#else
				Vector3d _partListPos = GetPosition (EditorPartList.Instance.transformTopLeft);
				Vector3d _footerPos = GetPosition (EditorPartList.Instance.footerTransform);
				float _partsPanelTrueWidth = EditorPanels.Instance.partsPanelWidth - PartCategorizer.Instance.scrollListMain.scrollList.viewableArea.x - PartCategorizer.Instance.scrollListSub.scrollList.viewableArea.x;
				return new Rect ((float)_partListPos.x +10, (float)_footerPos.y -20, _partsPanelTrueWidth -40, 30);
				#endif
			}
		}
		private static Rect RectOthers {
			get {
				Vector3d _prevPagePos = GetPosition (EditorPartList.Instance.prevPage.transform);
				Vector3d _nextPagePos = GetPosition (EditorPartList.Instance.nextPage.transform);
				return new Rect ((float)_prevPagePos.x +25, (float)_prevPagePos.y -10, (float)_nextPagePos.x - (float)_prevPagePos.x -55, 40);
			}
		}

		public static Vector3 GetPosition(Transform trans) {
			EZCameraSettings _uiCam = UIManager.instance.uiCameras.FirstOrDefault(c => (c.mask & (1 << trans.gameObject.layer)) != 0);
			if (_uiCam != null) {
				Vector3 _screenPos = _uiCam.camera.WorldToScreenPoint (trans.position);
				_screenPos.y = Screen.height - _screenPos.y;
				return _screenPos;
			}
			return Vector3d.zero;
		}

		internal static void Init() {
			#if !TINY
			BackTexture = GameDatabase.Instance.GetTexture (BackTexturePath, false);
			SaveTexture = GameDatabase.Instance.GetTexture (SaveTexturePath, false);
			DeleteTexture = GameDatabase.Instance.GetTexture (DeleteTexturePath, false);
			#endif
			TextField = HighLogic.Skin.textField;
			TextField.stretchWidth = true;
			TextField.fixedHeight = 20;
			TextField.alignment = TextAnchor.MiddleCenter;
			Ready = true;
			QuickSearch.Log ("GUI Init");
		}

		internal static void OnGUI() {
			GUI.skin = HighLogic.Skin;
			PartCategorizer.Category _currentFilter = QCategory.CurrentFilter;
			PartCategorizer.Category _currentSubCategory = QCategory.CurrentSubCategory;
			if (_currentFilter != null) {
				if (QCategory.isPartSearch) {
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
				DrawSearch ((_currentFilter.displayType == EditorPartList.State.PartsList ? RectPartsList : RectOthers));
				#if !TINY
				if (_currentFilter == QCategory.FilterPartSearch) {
					if (_currentFilter.displayType == EditorPartList.State.PartsList) {
						DrawButton (RectOthers);
					}
				}
				#endif
			}
		}

		private static void DrawSearch(Rect rectArea) {
			//string _Text = QSearch.CleanInput (QSearch.Text);
			GUILayout.BeginArea (rectArea);
			DrawSpace ();
			GUILayout.BeginVertical ();
			GUILayout.BeginHorizontal ();
			string _Text = GUILayout.TextField (QSearch.Text, TextField);
			_Text = CleanInput(_Text);
			if (_Text != QSearch.Text) {
				if (PartListTooltips.fetch.displayTooltip) {
					GameEvents.onTooltipDestroyRequested.Fire();
					PartListTooltips.fetch.HideTooltip ();
				}
				QSearch.Text = _Text;
				QCategory.Refresh ();
			}
			GUILayout.EndHorizontal ();
			GUILayout.EndVertical ();
			DrawSpace ();
			GUILayout.EndArea ();
		}

		#if !TINY	
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
		#endif
		private static void DrawSpace() {
			GUILayout.BeginVertical ();
			GUILayout.Space (5);
			GUILayout.EndVertical ();
		}

		internal static string CleanInput(string strIn) {
			// Replace invalid characters with empty strings. 
			return Regex.Replace(strIn, @"[^\w\.@-|&/\(\)\[\]\+?,;:/\*µ\^\$=\ ""]", string.Empty); 
		}
	}
}