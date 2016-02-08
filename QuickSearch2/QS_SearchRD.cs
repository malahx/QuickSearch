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
using UnityEngine;

namespace QuickSearch2 {
	internal class QSearchRD {
		internal static void Find(bool clean = false) {
			List<RDNode> _nodes = RDController.Instance.nodes;
			foreach (RDNode _node in _nodes) {
				RDTech _rdTech = _node.tech;
			 	UIStateToggleBtn _button = _node.graphics.button;
				if (!clean && _rdTech.partsAssigned.Find (aPart => QSearch.FindPart (aPart)) != null) {
					_button.SetColor(new Color (1f, 0f, 0f));
					continue;
				}
				_button.SetColor(new Color (1f, 1f, 1f));
			}
			QuickSearch2.Log ("QSearchRD.Find: " + QSearch.Text, true);
		}
	}
}