﻿using System.Activities.Presentation.Annotations;

namespace NativeApp.ViewModels
{
	using System.ComponentModel;
	using System.Runtime.CompilerServices;
	using Annotations;


	public class ViewModelBase : INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler PropertyChanged;

		[NotifyPropertyChangedInvocator]
		protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}