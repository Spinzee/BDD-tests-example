namespace Products.ControllerHelpers
{
    using System;
    using System.Collections.Generic;
    using Infrastructure;
    using Model.Common.CMSResponse;
    using Model.Constants;
    using Model.Energy;
    using Service.Mappers;
    using ServiceWrapper.ContentManagementService;

    public class ContentManagementControllerHelper : BaseControllerHelper, IContentManagementControllerHelper
    {
        private readonly IContentManagementAPIClient _contentManagementAPIClient;
        private readonly ISessionManager _sessionManager;
        private List<CMSEnergyContent> _cmsContentList;

        public ContentManagementControllerHelper(IContentManagementAPIClient contentManagementAPIClient, ISessionManager sessionManager)
        {
            Guard.Against<ArgumentException>(contentManagementAPIClient == null, $"{nameof(contentManagementAPIClient)} is null");
            Guard.Against<ArgumentException>(sessionManager == null, $"{nameof(sessionManager)} is null");
            _contentManagementAPIClient = contentManagementAPIClient;
            _sessionManager = sessionManager;
        }

        public List<CMSEnergyContent> GetCMSEnergyContentList()
        {
            _cmsContentList = _sessionManager.GetSessionDetails<List<CMSEnergyContent>>(SessionKeys.CMSList);

            if (_cmsContentList == null)
            {
                CMSResponseModel response = _contentManagementAPIClient.GetTariffContent().Result;
                _cmsContentList = ContentManagementServiceMapper.MapEntryListToCMSEnergyContentList(response.Entries);
                _sessionManager.SetSessionDetails(SessionKeys.CMSList, _cmsContentList);
            }

            return _cmsContentList;
        }
    }
}
