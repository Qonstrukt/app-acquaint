using System;
using System.Threading.Tasks;
using Acquaint.Data;
using Acquaint.Models;
using Acquaint.Util;
using Acquaint.ViewModels;
using CoreGraphics;
using Foundation;
using ReactiveUI;
using UIKit;
using System.Reactive.Disposables;
using System.Reactive.Linq;

namespace Acquaint.Native.iOS
{
	/// <summary>
	/// Acquaintance table view controller. The layout for this view controller is defined almost entirely in Main.storyboard.
	/// </summary>
	public partial class AcquaintanceTableViewController : ReactiveTableViewController<AcquaintanceTableViewModel>, IUIViewControllerPreviewingDelegate
	{
		// This constructor signature is required, for marshalling between the managed and native instances of this class.
		public AcquaintanceTableViewController(IntPtr handle) : base(handle)
		{
			RefreshControl = new UIRefreshControl();
		}

		// The ViewDidLoad() method is called when the view is first requested by the application.
		// The "async" keyword is added here to the override in order to allow other awaited async method calls inside the override to be called ascynchronously.
		public override void ViewDidLoad()
		{
			base.ViewDidLoad();

			ViewModel = new AcquaintanceTableViewModel();

			SetTableViewProperties();

			// override the back button text for AcquaintanceDetailViewController (the navigated-to view controller)
			NavigationItem.BackBarButtonItem = new UIBarButtonItem("List", UIBarButtonItemStyle.Plain, null);

			this.WhenActivated(disposable =>
			{
				this.WhenAnyValue(view => view.ViewModel.Acquaintances)
					.BindTo<AcquaintanceViewModel, AcquaintanceCell>(TableView, (NSString)"AcquaintanceCell", 60, null)
					.DisposeWith(disposable);

				this.BindCommand(ViewModel, vm => vm.RefreshAcquaintances, view => view.RefreshControl)
					.DisposeWith(disposable);

				ViewModel.RefreshAcquaintances.IsExecuting
						 .Where(isExecuting => !isExecuting)
						 .Subscribe(_ => RefreshControl.EndRefreshing())
						 .DisposeWith(disposable);

				ViewModel.RefreshAcquaintances.Execute()
						 .Subscribe()
						 .DisposeWith(disposable);
			});

			TableView.AddSubview(RefreshControl);
		}

		// The ViewDidAppear() override is called after the view has appeared on the screen.
		public override void ViewDidAppear(bool animated)
		{
			base.ViewDidAppear(animated);

			if (string.IsNullOrWhiteSpace(Settings.DataPartitionPhrase))
			{
				PerformSegue("PresentSetupViewControllerSegue", this);

				return;
			}
		}


		/// <summary>
		/// Sets some table view properties.
		/// </summary>
		void SetTableViewProperties()
		{
			TableView.AllowsSelection = true;

			TableView.RowHeight = 60;
		}


		// The PrepareForSegue() override is called when a segue has been activated, but before it executes.
		public override void PrepareForSegue(UIStoryboardSegue segue, NSObject sender)
		{
			// Determine segue action by segue identifier.
			// Note that these segues are defined in Main.storyboard.
			switch (segue.Identifier)
			{
				case "NewAcquaintanceSegue":
					// get the destination viewcontroller from the segue
					var acquaintanceEditViewController = segue.DestinationViewController as AcquaintanceEditViewController;
					// instantiate new Acquaintance and assign to viewcontroller
					acquaintanceEditViewController.Acquaintance = null;
					break;
				case "AcquaintanceDetailSegue":
					// the selected index path
					var indexPath = TableView.IndexPathForSelectedRow;
					// the index of the item in the collection that corresponds to the selected cell
					var itemIndex = indexPath.Row;
					// get the destination viewcontroller from the segue
					var acquaintanceDetailViewController = segue.DestinationViewController as AcquaintanceDetailViewController;
					// if the detaination viewcontrolller is not null
					if (acquaintanceDetailViewController != null && TableView.Source != null)
					{
						// set the acquaintance on the view controller
						acquaintanceDetailViewController.Acquaintance = ViewModel.Acquaintances[indexPath.Row].Acquaintance;
					}
					break;
			}
		}

		/// <summary>
		/// Called when the iOS interface environment changes. We're using it here to detect whether or not 3D Touch capabiliies are available on the device.
		/// </summary>
		/// <param name="previousTraitCollection">Previous trait collection.</param>
		public override void TraitCollectionDidChange(UITraitCollection previousTraitCollection)
		{
			base.TraitCollectionDidChange(previousTraitCollection);

			// If 3D Touch (ForceTouch) is available, then register a IUIViewControllerPreviewingDelegate, which in this case is this very class because we're implementing IUIViewControllerPreviewingDelegate.
			// You could put the IUIViewControllerPreviewingDelegate implementation in another class if you wanted to. Then you'd pass an instance of it, instead of "this".
			if (TraitCollection.ForceTouchCapability == UIForceTouchCapability.Available)
			{
				RegisterForPreviewingWithDelegate(this, View);
			}
		}

		/// <summary>
		/// Gets the view controller that will be displayed for a 3D Touch preview "peek".
		/// </summary>
		/// <returns>The view controller for preview.</returns>
		/// <param name="previewingContext">Previewing context.</param>
		/// <param name="location">Location.</param>
		public UIViewController GetViewControllerForPreview(IUIViewControllerPreviewing previewingContext, CoreGraphics.CGPoint location)
		{
			// Obtain the index path and the cell that was pressed.
			var indexPath = TableView.IndexPathForRowAtPoint(location);

			if (indexPath == null)
				return null;

			// get the cell that is being pressed for preview "peeking"
			var cell = TableView.CellAt(indexPath);

			if (cell == null)
				return null;

			// Create a detail view controller and set its properties.
			var detailViewController = Storyboard.InstantiateViewController("AcquaintanceDetailViewController") as AcquaintanceDetailViewController;

			if (detailViewController == null)
				return null;

			// set the acquaintance on the view controller
			detailViewController.Acquaintance = ViewModel.Acquaintances[indexPath.Row].Acquaintance;

			// set the frame on the screen that will NOT be blurred out during the preview "peek"
			previewingContext.SourceRect = cell.Frame;

			return detailViewController;
		}

		/// <summary>
		/// Commits the view controller that will displayed when a 3D Touch gesture is fully depressed, beyond just the preview.
		/// </summary>
		/// <param name="previewingContext">Previewing context.</param>
		/// <param name="viewControllerToCommit">View controller to commit.</param>
		public void CommitViewController(IUIViewControllerPreviewing previewingContext, UIViewController viewControllerToCommit)
		{
			// Show the view controller that is being preview "peeked".
			// Instead, you could do whatever you want here, such as commit some other view controller than the one that is being "peeked".
			ShowViewController(viewControllerToCommit, this);
		}
	}
}
