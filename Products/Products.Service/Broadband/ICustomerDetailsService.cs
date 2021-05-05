namespace Products.Service.Broadband
{
    using System;
    using WebModel.ViewModels.Broadband;

    public interface ICustomerDetailsService
    {
        PersonalDetailsViewModel GetPersonalDetailsViewModel();

        void SetPersonalDetailsViewModel(PersonalDetailsViewModel personalDetailsViewModel);

        ContactDetailsViewModel GetContactDetailsViewModel();

        void SetContactDetailsViewModel(ContactDetailsViewModel contactDetailsViewModel, Guid userId);

        TransferYourNumberViewModel GetTransferYourNumberViewModel();

        void SetTransferYourNumberViewModel(TransferYourNumberViewModel model);

        TransferYourNumberViewModel GetUpdatedTransferYourNumberViewModel(TransferYourNumberViewModel model);
    }
}