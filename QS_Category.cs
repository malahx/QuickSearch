/* 
QuickSearch
Copyright 2016 Malah

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
using System.Collections.Generic;
using RUI.Icons.Selectable;
using UnityEngine;

namespace QuickSearch {
	internal class QCategory : QCategorizer {

		internal static bool Ready = false;

		internal static bool isPartSearch {
			get {
				#if !TINY
				return CurrentFilter == QCategory.FilterPartSearch || CurrentFilter.displayType == EditorPartList.State.SubassemblyList;
				#else
				return CurrentSubCategory == QCategory.SubCategoryPartSearch || CurrentFilter.displayType == EditorPartList.State.SubassemblyList;
				#endif
			}
		}

		internal static void Init() {
			IconTexture = GameDatabase.Instance.GetTexture (IconTexturePath, false);
			IconSelectedTexture = GameDatabase.Instance.GetTexture (IconSelectedTexturePath, false);
			Icon = new Icon (QuickSearch.MOD, IconTexture, IconSelectedTexture);

			#if !TINY
			FilterPartSearch = PartCategorizer.AddCustomFilter(QuickSearch.MOD, Icon, new Color (0.88f, 0.53f, 0.53f));
			FilterPartSearch.displayType = EditorPartList.State.PartsList;
			#else
			FilterPartSearch = FilterByFunctions;
			#endif

			SubCategoryPartSearch =  PartCategorizer.AddCustomSubcategoryFilter(FilterPartSearch, "New Search", Icon,  part => QSearch.FindPart (part));
			SubCategoryPartSearch.displayType = EditorPartList.State.PartsList;
			#if TINY
			SubCategoryPartSearch.button.activeButton.SetColor (new Color (0.88f, 0.53f, 0.53f));
			RUIToggleButtonTyped button = FilterPartSearch.button.activeButton;
			button.SetFalse(button, RUIToggleButtonTyped.ClickType.FORCED);
			button.SetTrue(button, RUIToggleButtonTyped.ClickType.FORCED);
			#endif
			List<PartCategorizer.Category> _categories = PartCategorizer.Instance.categories;
			foreach (PartCategorizer.Category _category in _categories) {
				if (_category.displayType == EditorPartList.State.SubassemblyList) {
					PartCategorizer.Category _subcategory = _category.subcategories[0];
					_subcategory.exclusionFilterSubassembly = new EditorPartListFilter<ShipTemplate> (QuickSearch.MOD, s => QSearch.FindSubassembly (s));
				}
			}
			#if !TINY
			Populate ();
			#endif
			Ready = true;
			QuickSearch.Log ("Category Init");
		}
		#if !TINY	
		internal static void GoToLastCategory() {
			QSearch.Text = string.Empty;
			if (lastIsAdvancedMode) {
				PartCategorizer.Instance.SetAdvancedMode ();
			} else {
				PartCategorizer.Instance.SetSimpleMode ();
			}
			if (lastFilter != null) {
				RUIToggleButtonTyped _btn = lastFilter.button.activeButton;
				if (_btn.State != RUIToggleButtonTyped.ButtonState.TRUE) {
					_btn.SetTrue (_btn, RUIToggleButtonTyped.ClickType.FORCED, true);
				}
			}
			if (lastSubCategory != null) {
				RUIToggleButtonTyped _btn = lastSubCategory.button.activeButton;
				if (_btn.State != RUIToggleButtonTyped.ButtonState.TRUE) {
					_btn.SetTrue (_btn, RUIToggleButtonTyped.ClickType.FORCED, true);
				}
			}
			QuickSearch.Log("Back to the last Category");
		}

		internal static void Bookmark(string text) {
			if (!Exists (text)) {
				AddSubCategory (text);
			} else {
				DeleteSubCategory (text);
			}
		}
		#endif
	}
}