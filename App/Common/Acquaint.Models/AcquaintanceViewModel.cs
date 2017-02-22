using System;
using ReactiveUI;

namespace Acquaint.Models
{
	public class AcquaintanceViewModel : ReactiveObject
	{
		public readonly Acquaintance Acquaintance;

		public string DataPartitionId { get; set; }

		string _FirstName;
		public string FirstName
		{
			get { return _FirstName; }
			set { this.RaiseAndSetIfChanged(ref _FirstName, value); }
		}

		string _LastName;
		public string LastName
		{
			get { return _LastName; }
			set { this.RaiseAndSetIfChanged(ref _LastName, value); }
		}

		string _Company;
		public string Company
		{
			get { return _Company; }
			set { this.RaiseAndSetIfChanged(ref _Company, value); }
		}

		string _JobTitle;
		public string JobTitle
		{
			get { return _JobTitle; }
			set { this.RaiseAndSetIfChanged(ref _JobTitle, value); }
		}

		string _Email;
		public string Email
		{
			get { return _Email; }
			set { this.RaiseAndSetIfChanged(ref _Email, value); }
		}

		string _Phone;
		public string Phone
		{
			get { return _Phone; }
			set { this.RaiseAndSetIfChanged(ref _Phone, value); }
		}

		string _Street;
		public string Street
		{
			get { return _Street; }
			set { this.RaiseAndSetIfChanged(ref _Street, value); }
		}

		string _City;
		public string City
		{
			get { return _City; }
			set { this.RaiseAndSetIfChanged(ref _City, value); }
		}

		string _PostalCode;
		public string PostalCode
		{
			get { return _PostalCode; }
			set { this.RaiseAndSetIfChanged(ref _PostalCode, value); }
		}

		string _State;
		public string State
		{
			get { return _State; }
			set { this.RaiseAndSetIfChanged(ref _State, value); }
		}

		string _PhotoUrl;
		public string PhotoUrl
		{
			get { return _PhotoUrl; }
			set { this.RaiseAndSetIfChanged(ref _PhotoUrl, value); }
		}

		public string SmallPhotoUrl => PhotoUrl;

		string _AddressString;
		public string AddressString
		{
			get { return _AddressString; }
			private set { this.RaiseAndSetIfChanged(ref _AddressString, value); }
		}

		string _DisplayName;
		public string DisplayName
		{
			get { return _DisplayName; }
			private set { this.RaiseAndSetIfChanged(ref _DisplayName, value); }
		}

		string _DisplayLastNameFirst;
		public string DisplayLastNameFirst
		{
			get { return _DisplayLastNameFirst; }
			private set { this.RaiseAndSetIfChanged(ref _DisplayLastNameFirst, value); }
		}

		string _StatePostal;
		public string StatePostal
		{
			get { return _StatePostal; }
			private set { this.RaiseAndSetIfChanged(ref _StatePostal, value); }
		}

		public AcquaintanceViewModel(Acquaintance acquaintance)
		{
			Acquaintance = acquaintance;

			DataPartitionId = acquaintance.DataPartitionId;
			FirstName = acquaintance.FirstName;
			LastName = acquaintance.LastName;
			Company = acquaintance.Company;
			JobTitle = acquaintance.JobTitle;
			Email = acquaintance.Email;
			Phone = acquaintance.Phone;
			Street = acquaintance.Street;
			City = acquaintance.City;
			PostalCode = acquaintance.PostalCode;
			State = acquaintance.State;
			PhotoUrl = acquaintance.PhotoUrl;

			this.WhenAnyValue(vm => vm.FirstName, vm => vm.LastName, (firstName, lastName) => $"{firstName} {lastName}")
				.BindTo(this, vm => vm.DisplayName);

			this.WhenAnyValue(vm => vm.FirstName, vm => vm.LastName, (firstName, lastName) => $"{lastName}, {firstName}")
				.BindTo(this, vm => vm.DisplayLastNameFirst);

			this.WhenAnyValue(vm => vm.State, vm => vm.PostalCode, (state, postalCode) => State + " " + PostalCode)
				.BindTo(this, vm => vm.StatePostal);

			this.WhenAnyValue(vm => vm.Street, vm => vm.City, vm => vm.State, vm => vm.PostalCode,
							  (street, city, state, postalCode) => $"{street} {city}, {state} {postalCode}")
				.BindTo(this, vm => vm.AddressString);
		}

		public override string ToString()
		{
			return DisplayName;
		}
	}
}
