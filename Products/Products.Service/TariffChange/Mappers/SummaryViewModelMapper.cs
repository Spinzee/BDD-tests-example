namespace Products.Service.TariffChange.Mappers
{
    using System.Collections.Generic;
    using System.Linq;
    using Core;
    using Infrastructure.Extensions;
    using Products.Model.TariffChange.Customers;
    using Products.Model.TariffChange.Enums;
    using Products.WebModel.Resources.TariffChange;
    using Products.WebModel.ViewModels.TariffChange;

    public class SummaryViewModelMapper
    {
        public static TariffSummaryViewModel MapSummary(Customer customer, CTCJourneyType ctcJourneyType)
        {
            var model = new TariffSummaryViewModel
            {
                CTCJourneyType = ctcJourneyType,
                SelectedTariff = new AvailableTariff
                {
                    Name = customer.CustomerSelectedTariff.Name,
                    Tagline = customer.CustomerSelectedTariff.Tagline,
                    ProjectedAnnualCost = customer.CustomerSelectedTariff.ProjectedAnnualCost,
                    ProjectedMonthlyCost = customer.CustomerSelectedTariff.ProjectedMonthlyCost,
                    TermsAndConditionsPdfLinks = customer.CustomerSelectedTariff.TermsAndConditionsPdfLink,
                    TariffGroup = customer.CustomerSelectedTariff.TariffGroup,
                    ElectricityDetails = !customer.HasElectricityAccount() ? null : new TariffInformationLabel
                    {
                        AnnualCost = customer.ElectricityAccount.SelectedTariff.AnnualCost,
                        MonthlyCost = customer.ElectricityAccount.SelectedTariff.MonthlyCost,
                        Supplier = customer.ElectricityAccount.SelectedTariff.Supplier,
                        TariffName = customer.ElectricityAccount.SelectedTariff.TariffName,
                        TariffType = customer.ElectricityAccount.SelectedTariff.TariffType,
                        PaymentMethod = customer.ElectricityAccount.SelectedTariff.PaymentMethod,
                        UnitRate1 = customer.ElectricityAccount.SelectedTariff.UnitRate1,
                        UnitRate2 = customer.ElectricityAccount.SelectedTariff.UnitRate2,
                        DayOrStandardLabel = customer.ElectricityAccount.SelectedTariff.DayOrStandardLabel,
                        NightOrOffPeakLabel = customer.ElectricityAccount.SelectedTariff.NightOrOffPeakLabel,
                        StandingCharge = customer.ElectricityAccount.SelectedTariff.StandingCharge,
                        TariffEndsOn = customer.ElectricityAccount.SelectedTariff.TariffEndsOn,
                        PriceGuaranteedUntil = customer.ElectricityAccount.SelectedTariff.PriceGuaranteedUntil,
                        ExitFees = customer.ElectricityAccount.SelectedTariff.ExitFees,
                        DiscountsAndAdditionalCharges = customer.ElectricityAccount.SelectedTariff.DiscountsAndAdditionalCharges,
                        AdditionalProductsAndServicesIncluded = customer.ElectricityAccount.SelectedTariff.AdditionalProductsAndServicesIncluded,
                        TCR = customer.ElectricityAccount.SelectedTariff.TCR
                    },
                    GasDetails = !customer.HasGasAccount() ? null : new TariffInformationLabel
                    {
                        AnnualCost = customer.GasAccount.SelectedTariff.AnnualCost,
                        MonthlyCost = customer.GasAccount.SelectedTariff.MonthlyCost,
                        Supplier = customer.GasAccount.SelectedTariff.Supplier,
                        TariffName = customer.GasAccount.SelectedTariff.TariffName,
                        TariffType = customer.GasAccount.SelectedTariff.TariffType,
                        PaymentMethod = customer.GasAccount.SelectedTariff.PaymentMethod,
                        UnitRate1 = customer.GasAccount.SelectedTariff.UnitRate1,
                        StandingCharge = customer.GasAccount.SelectedTariff.StandingCharge,
                        TariffEndsOn = customer.GasAccount.SelectedTariff.TariffEndsOn,
                        PriceGuaranteedUntil = customer.GasAccount.SelectedTariff.PriceGuaranteedUntil,
                        ExitFees = customer.GasAccount.SelectedTariff.ExitFees,
                        DiscountsAndAdditionalCharges = customer.GasAccount.SelectedTariff.DiscountsAndAdditionalCharges,
                        AdditionalProductsAndServicesIncluded = customer.GasAccount.SelectedTariff.AdditionalProductsAndServicesIncluded,
                        TCR = customer.GasAccount.SelectedTariff.TCR
                    }
                },
                DayOfPaymentEachMonth = customer.IterableAccounts().FirstOrDefault(c => c.PaymentDetails.IsMonthlyDirectDebit)?.PaymentDetails.DirectDebitDay.AppendPaymentDaySuffix(),
                FuelType = customer.GetCustomerFuelType(),
                HasAnyDirectDebitAccount = customer.HasElectricityAccount() && customer.ElectricityAccount.PaymentDetails.IsDirectDebit || customer.HasGasAccount() && customer.GasAccount.PaymentDetails.IsDirectDebit,
                NewTariffStartDate = customer.CustomerSelectedTariff.EffectiveDate
            };

            bool variableDirectDebit = false;
            bool monthlyDirectDebit = false;

            if (customer.HasElectricityAccount())
            {
                if (customer.ElectricityAccount.PaymentDetails.IsDirectDebit)
                {
                    variableDirectDebit = customer.ElectricityAccount.PaymentDetails.IsVariableDirectDebit;
                    monthlyDirectDebit = customer.ElectricityAccount.PaymentDetails.IsMonthlyDirectDebit;
                    model.ElectricityAmount = customer.ElectricityAccount.PaymentDetails.IsVariableDirectDebit ? "Payment as per bill amount" : customer.ElectricityAccount.PaymentDetails.DirectDebitAmount.ToPounds();
                }
                else
                {
                    model.ElectricityAmount = "Quarterly on receipt of your bill";
                }
            }

            if (customer.HasGasAccount())
            {
                if (customer.GasAccount.PaymentDetails.IsDirectDebit)
                {
                    variableDirectDebit = customer.GasAccount.PaymentDetails.IsVariableDirectDebit || variableDirectDebit;
                    monthlyDirectDebit = customer.GasAccount.PaymentDetails.IsMonthlyDirectDebit || monthlyDirectDebit;
                    model.GasAmount = customer.GasAccount.PaymentDetails.IsVariableDirectDebit ? "Payment as per bill amount" : customer.GasAccount.PaymentDetails.DirectDebitAmount.ToPounds();
                }
                else
                {
                    model.GasAmount = "Quarterly on receipt of your bill";
                }
            }

            if (customer.IsDualFuel())
            {
                if (monthlyDirectDebit && variableDirectDebit)
                {
                    model.Title = "Payment details";
                    model.Header1 = customer.ElectricityAccount.PaymentDetails.IsMonthlyDirectDebit
                        ? TariffSummary_Resources.AdditionalInfoIntroMonthlyDD : TariffSummary_Resources.AdditionalInfoIntroVariable;
                    model.Header2 = customer.GasAccount.PaymentDetails.IsMonthlyDirectDebit
                        ? TariffSummary_Resources.AdditionalInfoIntroMonthlyDD : TariffSummary_Resources.AdditionalInfoIntroVariable;
                    model.ElectricityAmountLabel = "Electricity";
                    model.GasAmountLabel = "Gas";
                    model.ElectricityFrequency = customer.ElectricityAccount.PaymentDetails.IsMonthlyDirectDebit
                        ? $"{customer.ElectricityAccount.PaymentDetails.DirectDebitDay.AppendPaymentDaySuffix()} of each month"
                        : "Quarterly as per date on bill";
                    model.GasFrequency = customer.GasAccount.PaymentDetails.IsMonthlyDirectDebit
                        ? $"{customer.GasAccount.PaymentDetails.DirectDebitDay.AppendPaymentDaySuffix()} of each month"
                        : "Quarterly as per date on bill";
                }
                else
                {
                    if (monthlyDirectDebit)
                    {
                        bool allDirectDebit = customer.IterableAccounts().All(ca => ca.PaymentDetails.IsMonthlyDirectDebit);
                        model.Title = allDirectDebit ? "Direct Debit payment details" : "Payment details";
                        model.Header1 = allDirectDebit ? TariffSummary_Resources.AdditionalInfoIntroMonthly : TariffSummary_Resources.AdditionalInfoIntroMonthlyDD;
                        model.ElectricityAmountLabel = "Electricity per month";
                        model.GasAmountLabel = "Gas per month";
                        model.GasAndElectricityFrequencyLabel = "Date of payment each month";
                        model.GasAndElectricityFrequency = customer.IterableAccounts().FirstOrDefault(ca => ca.PaymentDetails.IsMonthlyDirectDebit)?.PaymentDetails.DirectDebitDay.AppendPaymentDaySuffix();
                    }
                    else if (variableDirectDebit)
                    {
                        model.Title = "Payment details";
                        model.Header1 = TariffSummary_Resources.AdditionalInfoIntroVariableDD;
                        model.ElectricityAmountLabel = "Electricity";
                        model.GasAmountLabel = "Gas";
                        if (customer.IterableAccounts().All(ca => ca.PaymentDetails.IsVariableDirectDebit))
                        {
                            model.Title = "Direct Debit payment details";
                            model.Header1 = TariffSummary_Resources.AdditionalInfoIntroVariable;
                            model.GasAndElectricityFrequencyLabel = "Frequency";
                            model.GasAndElectricityFrequency = "Quarterly as per date on bill";
                        }
                        else if (customer.ElectricityAccount.PaymentDetails.IsVariableDirectDebit)
                        {
                            model.ElectricityFrequency = "Quarterly as per date on bill";
                        }
                        else
                        {
                            model.GasFrequency = "Quarterly as per date on bill";
                        }
                    }
                }
            }
            else
            {
                model.Title = "Direct Debit payment details";
                if (customer.HasElectricityAccount())
                {
                    model.ElectricityAmountLabel = "Electricity";
                    model.ElectricityAmount = customer.ElectricityAccount.PaymentDetails.IsMonthlyDirectDebit ? customer.ElectricityAccount.PaymentDetails.DirectDebitAmount.ToPounds() : "Payment as per bill amount";
                }
                else
                {
                    model.GasAmountLabel = "Gas";
                    model.GasAmount = customer.GasAccount.PaymentDetails.IsMonthlyDirectDebit ? customer.GasAccount.PaymentDetails.DirectDebitAmount.ToPounds() : "Payment as per bill amount";
                }
                if (monthlyDirectDebit)
                {
                    model.Header1 = TariffSummary_Resources.AdditionalInfoIntroMonthly;
                    if (model.ElectricityAmountLabel != null)
                    {
                        model.ElectricityAmountLabel += " per month";
                    }
                    if (model.GasAmountLabel != null)
                    {
                        model.GasAmountLabel += " per month";
                    }
                    model.GasAndElectricityFrequencyLabel = "Date of payment each month";
                    model.GasAndElectricityFrequency = customer.IterableAccounts().FirstOrDefault()?.PaymentDetails.DirectDebitDay.AppendPaymentDaySuffix();
                }
                else if (variableDirectDebit)
                {
                    model.Header1 = TariffSummary_Resources.AdditionalInfoIntroVariable;
                    model.GasAndElectricityFrequencyLabel = "Frequency";
                    model.GasAndElectricityFrequency = "Quarterly as per date on bill";
                }
            }

            model.ProgressBarViewModel = GetProgressViewModel(ctcJourneyType);
            return model;
        }

        private static ProgressBarViewModel GetProgressViewModel(CTCJourneyType ctcJourneyType)
        {
            switch (ctcJourneyType)
            {
                case CTCJourneyType.PreLogIn:
                    return new ProgressBarViewModel
                    {
                        Sections = new List<ProgressBarSection>
                            {
                                new ProgressBarSection
                                {
                                    Text = ProgressBar_Resource.UserDetailsSectionHeader,
                                    Status = ProgressBarStatus.Done
                                },
                                new ProgressBarSection
                                {
                                    Text = ProgressBar_Resource.TariffChoiceSectionHeader,
                                    Status = ProgressBarStatus.Done
                                },
                                new ProgressBarSection
                                {
                                    Text = ProgressBar_Resource.EmailAddressSectionHeader,
                                    Status = ProgressBarStatus.Done
                                },
                                new ProgressBarSection
                                {
                                    Text = ProgressBar_Resource.SummarySectionHeader,
                                    Status = ProgressBarStatus.Active
                                }
                            }
                    };

                case CTCJourneyType.PostLogInWithMultipleSites:
                    return new ProgressBarViewModel
                    {
                        Sections = new List<ProgressBarSection>
                            {
                                new ProgressBarSection
                                {
                                    Text = ProgressBar_Resource.SelectAddressSectionHeader,
                                    Status = ProgressBarStatus.Done
                                },
                                new ProgressBarSection
                                {
                                    Text = ProgressBar_Resource.TariffChoiceSectionHeader,
                                    Status = ProgressBarStatus.Done
                                },
                                new ProgressBarSection
                                {
                                    Text = ProgressBar_Resource.SummarySectionHeader,
                                    Status = ProgressBarStatus.Active
                                }
                            }
                    };

                case CTCJourneyType.PostLogInWithNoAccounts:
                    return new ProgressBarViewModel
                    {
                        Sections = new List<ProgressBarSection>
                            {
                                new ProgressBarSection
                                {
                                    Text = ProgressBar_Resource.UserDetailsSectionHeader,
                                    Status = ProgressBarStatus.Done
                                },
                                new ProgressBarSection
                                {
                                    Text = ProgressBar_Resource.TariffChoiceSectionHeader,
                                    Status = ProgressBarStatus.Done
                                },
                                new ProgressBarSection
                                {
                                    Text = ProgressBar_Resource.SummarySectionHeader,
                                    Status = ProgressBarStatus.Active
                                }
                            }
                    };

                case CTCJourneyType.PostLogInWithSingleSite:
                    return new ProgressBarViewModel
                    {
                        Sections = new List<ProgressBarSection>
                            {
                                new ProgressBarSection
                                {
                                    Text = ProgressBar_Resource.TariffChoiceSectionHeader,
                                    Status = ProgressBarStatus.Done
                                },
                                new ProgressBarSection
                                {
                                    Text = ProgressBar_Resource.SummarySectionHeader,
                                    Status = ProgressBarStatus.Active
                                }
                            }
                    };
            }

            return null;
        }
    }
}