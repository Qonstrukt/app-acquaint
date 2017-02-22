using System;
using System.Threading.Tasks;
using Acquaint.Data;
using Acquaint.Models;
using Acquaint.Util;
using FFImageLoading;
using FFImageLoading.Transformations;
using UIKit;
using ReactiveUI;

namespace Acquaint.Native.iOS
{
	/// <summary>
	/// Acquaintance cell. The layout for this Cell is defined almost entirely in Main.storyboard.
	/// </summary>
	public partial class AcquaintanceCell : ReactiveTableViewCell<AcquaintanceViewModel>
	{
		// This constructor signature is required, for marshalling between the managed and native instances of this class.
		public AcquaintanceCell(IntPtr handle) : base(handle) { }

		public override void AwakeFromNib()
		{
			base.AwakeFromNib();

			// set disclousure indicator accessory for the cell
			Accessory = UITableViewCellAccessory.DisclosureIndicator;

			this.OneWayBind(ViewModel, vm => vm.DisplayLastNameFirst, view => view.NameLabel.Text);
			this.OneWayBind(ViewModel, vm => vm.Company, view => view.CompanyLabel.Text);
			this.OneWayBind(ViewModel, vm => vm.JobTitle, view => view.JobTitleLabel.Text);

			this.WhenAnyValue(view => view.ViewModel.SmallPhotoUrl)
				.Subscribe(photoUrl =>
				{
					ImageService.Instance
						.LoadUrl(photoUrl, TimeSpan.FromHours(Settings.ImageCacheDurationHours))  // get the image from a URL
						.LoadingPlaceholder("placeholderProfileImage.png")                                          // specify a placeholder image
						.Transform(new CircleTransformation())                                                      // transform the image to a circle
						.Error(e => System.Diagnostics.Debug.WriteLine(e.Message))
						.Into(ProfilePhotoImageView);
				});
		}
	}
}
