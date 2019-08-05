using System;
using Newtonsoft.Json;
using SnapIt.Library.Controls;

namespace SnapIt.Library.Entities
{
	public class Layout : Bindable
	{
		private string name;
		private LayoutArea layoutArea;

		public Guid Guid { get; set; }
		[JsonIgnore]
		public bool IsSaved { get; set; }

		public string Name
		{
			get => name;
			set
			{
				IsSaved = false;
				SetProperty(ref name, value);
			}
		}

		public LayoutArea LayoutArea { get => layoutArea; set => SetProperty(ref layoutArea, value); }

		public void GenerateLayoutArea(SnapArea snapArea)
		{
			LayoutArea = new LayoutArea();
			snapArea.GetLayoutAreas(LayoutArea);
		}
	}
}