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
using System.Collections.Generic;
using UnityEngine;

namespace QuickSearch {
	internal class QCategory : QCategorizer {

		internal static bool Ready = false;

		internal static void Init() {
			IconTexture = GameDatabase.Instance.GetTexture (IconTexturePath, false);
			IconSelectedTexture = GameDatabase.Instance.GetTexture (IconSelectedTexturePath, false);
			Icon = new PartCategorizer.Icon (Quick.MOD, IconTexture, IconSelectedTexture);

			FilterPartSearch = PartCategorizer.AddCustomFilter(Quick.MOD, Icon, new Color (0.88f, 0.53f, 0.53f));
			FilterPartSearch.displayType = EditorPartList.State.PartsList;
			SubCategoryPartSearch =  PartCategorizer.AddCustomSubcategoryFilter(FilterPartSearch, "New Search", Icon,  part => QSearch.FindPart (part));
			SubCategoryPartSearch.displayType = EditorPartList.State.PartsList;

			List<PartCategorizer.Category> _categories = PartCategorizer.Instance.categories;
			foreach (PartCategorizer.Category _category in _categories) {
				if (_category.displayType == EditorPartList.State.SubassemblyList) {
					PartCategorizer.Category _subcategory = _category.subcategories[0];
					_subcategory.exclusionFilterSubassembly = new EditorPartListFilter<ShipTemplate> (Quick.MOD, s => QSearch.FindSubassembly (s));
				}
			}

			Populate ();
			Ready = true;
			Quick.Log ("Category Init");
		}

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
			Quick.Log("Back to the last Category");
		}

		internal static void Bookmark(string text) {
			if (!Exists (text)) {
				AddSubCategory (text);
			} else {
				DeleteSubCategory (text);
			}
		}
	}
}