namespace CustomEditor.Models
{
	public static class Constants
	{
		/// <summary>
		/// Filter related to image extensions
		/// </summary>
		public static readonly string ImagesFilter = "Image files (*.png;*.jpeg;*jpg)|*.png;*.jpeg;*jpg|All files (*.*)|*.*";

		/// <summary>
		/// Filter related to current project extension
		/// </summary>
		public static readonly string ProjectTypeFilter = "Canvas files (*.aer)|*.aer";

		/// <summary>
		/// Lower bound for compare double values
		/// </summary>
		public static readonly double MinComparableThreshold = 1e-5;
	}
}
