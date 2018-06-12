using System.Collections.Generic;
namespace ASA.Core.Interfaces
{
    public interface IGatewayService
    {
        GovTalkMessage CreateGovTalkMessage(Sender sender, Business business, PeriodData periodData, VAT100 vatData, bool testInLive);
        GovTalkMessage CreateGovTalkMessageForAuthCheck(Sender sender, Business business, bool testInLive);
        GovTalkMessage CreateGovTalkMessageForPollRequest(string correlationId, string pollUri, bool testInLive);
        GovTalkMessage CreateGovTalkMessageForDeleteRequest(string correlationId, bool testInLive);
        GovTalkMessage CreateGovTalkMessageForDataRequest(string senderId, string senderValue, bool testInLive);
        HMRCResponse SendHMRCMessage(string message, string uri);
        //IRenvelope CreateIRenvelopeBody(Sender sender, Business business, PeriodData periodData, VATDeclarationRequest vatData);
        HMRCResponse Save(HMRCResponse response);
        void UpdatePeriodStatus(PeriodData periodInfo);
        void AddNextQuaterPeriod(PeriodData period);
        void UpdateEntities(Business business);
        void AddPeriodData(PeriodData data);
        IList<PeriodData> GetSubmissions(int BusinessId);
    }
}
