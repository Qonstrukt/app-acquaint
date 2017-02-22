using System;
using System.Collections.Generic;
using System.Reactive;
using System.Threading.Tasks;
using Acquaint.Abstractions;
using Acquaint.Models;
using Microsoft.Practices.ServiceLocation;
using ReactiveUI;
using System.Reactive.Linq;
using System.Linq;

namespace Acquaint.ViewModels
{
	public class AcquaintanceTableViewModel : ReactiveObject
	{
		public ReactiveCommand<Unit, IEnumerable<Acquaintance>> RefreshAcquaintances;

		ReactiveList<AcquaintanceViewModel> _Acquaintances;
		public ReactiveList<AcquaintanceViewModel> Acquaintances
		{
			get { return _Acquaintances; }
			set { this.RaiseAndSetIfChanged(ref _Acquaintances, value); }
		}


		IDataSource<Acquaintance> _DataSource;


		public AcquaintanceTableViewModel()
		{
			Acquaintances = new ReactiveList<AcquaintanceViewModel>();

			RefreshAcquaintances = ReactiveCommand.CreateFromTask(() => GetAcquaintances());
			RefreshAcquaintances.ThrownExceptions.Subscribe();
			RefreshAcquaintances.Select(list => list.Select(acquaintance => new AcquaintanceViewModel(acquaintance)))
								.Select(list => new ReactiveList<AcquaintanceViewModel>(list))
								// .BindTo (this, vm => vm.Acquaintances);
								.Subscribe(list =>
								{
									Acquaintances.Clear();
									Acquaintances.AddRange(list);
								});
		}

		Task<IEnumerable<Acquaintance>> GetAcquaintances()
		{
			_DataSource = ServiceLocator.Current.GetInstance<IDataSource<Acquaintance>>();

			return _DataSource.GetItems();
		}

		void GetAcquaintancesExeceptions(Exception exception)
		{
			System.Diagnostics.Debug.WriteLine(exception.Message);
		}
	}
}
