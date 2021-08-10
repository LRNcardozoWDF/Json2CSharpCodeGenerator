using Common.Business.Service.Interface;
using Common.Business.Service.Mapping;
using Common.Models;
using Common.Models.Output;
using ParticularesServices.Services.Payment.Implementations;
using Payment.Business.Models.Output;
using Payment.Models.Request;
using Payments.Business.Models.Output;
using Payments.Business.Services.Implementations;
using Payments.Business.Services.Interfaces;
using Payments.Business.Services.Mappings.Request;
using Payments.Business.Services.Mappings.Result;
using Payments.Business.Validation.Request;
using Payments.Models.Request;
using Payments.Models.Result;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Payments.Business.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly IServiceManager serviceManager;

        public PaymentService(IServiceManager sm)
        {
            serviceManager = sm;
        }

        public async Task<bool> CreatePaymentCommunicationViacardPreProcessed(CreatePaymentCommunicationViacardPreProcessedRequest viacardPreProcessedRequest)
        {
            var request = PaymentServiceRequestMapping.CreatePaymentCommunicationViacardPreProcessed(viacardPreProcessedRequest);
            var result = (bool)await serviceManager.Invoke<CreatePaymentCommunicationViacardPreProcessedService>(request);
            return result;
        }

        public async Task CreateSecureContact(CreateSecureContactRequest createSecureContactRequest)
        {
            var request = PaymentServiceRequestMapping.CreateSecureContact(createSecureContactRequest, serviceManager.SessionManager.GetUserName());
            await serviceManager.Invoke<CreateSecureContactService>(request);
        }

        public async Task<string> DisableDebitAuthorization(DisableDebitAuthorizationRequest request)
        {
            var input = PaymentServiceRequestMapping.DisableDebitAuthorization(request);
            var output = (DisableDebitAuthorizationOutput)await serviceManager.Invoke<DisableDebitAuthorizationService>(input);

            return output.DebitAuthorizationNumber;
        }

        public async Task<IList<GetAllDebitAuthorizationResult>> GetAllDebitAuthorization(string operationType, string payerAccount)
        {
            var input = PaymentServiceRequestMapping.GetAllDebitAuthorization(operationType, payerAccount);
            var output = (GetAllDebitAuthorizationOutput)await serviceManager.Invoke<GetAllDebitAuthorizationService>(input);

            return PaymentServiceResultMapping.GetAllDebitAuthorization(output);
        }

        public async Task<GetDebitAuthorizationDetailResult> GetDebitAuthorizationDetail(GetDebitAuthorizationDetailRequest request)
        {
            var input = PaymentServiceRequestMapping.GetDebitAuthorizationDetail(request);
            var output = (GetDebitAuthorizationDetailOutput)await serviceManager.Invoke<GetDebitAuthorizationDetailService>(input);

            return PaymentServiceResultMapping.GetDebitAuthorizationDetail(output);
        }

        public async Task<ICollection<OperationLog>> GetOperationLog(string accountNumber, DateTime endDate, DateTime startDate, int maximumRecordsToReturn)
        {
            var request = PaymentServiceRequestMapping.GetOperationLog(accountNumber, endDate, startDate, maximumRecordsToReturn);
            var operationLogOutput = (OperationLogOutput)await serviceManager.Invoke<GetOperationLogService>(request);
            return PaymentServiceResultMapping.GetOperationLog(operationLogOutput);
        }

        public async Task<ICollection<OperationLog>> GetOperationTopLog(string accountNumber, int maximumRecordsToReturn)
        {
            var request = PaymentServiceRequestMapping.GetOperationTopLog(accountNumber, maximumRecordsToReturn);
            var operationLogOutput = (OperationLogOutput)await serviceManager.Invoke<GetOperationNLogService>(request);
            return PaymentServiceResultMapping.GetOperationLog(operationLogOutput);
        }

        public async Task<ICollection<PaymentCommunicationViacardEntityInformation>> GetPaymentCommunicationViacardEntityInformation()
        {
            var getPaymentCommunicationViacardEntityInformationOutput = (GetPaymentCommunicationViacardEntityInformationOutput)await serviceManager.Invoke<GetPaymentCommunicationViacardEntityInformationService>();
            return PaymentServiceResultMapping.GetPaymentCommunicationViacardEntityInformation(getPaymentCommunicationViacardEntityInformationOutput);
        }

        public async Task<GetPaymentCommunicationViacardInformationResult> GetPaymentCommunicationViacardInformation(string entityId)
        {
            var getPaymentCommunicationViacardInformationOutput = (GetPaymentCommunicationViacardInformationOutput)await serviceManager.Invoke<GetPaymentCommunicationViacardInformationService>(entityId);
            return PaymentServiceResultMapping.GetPaymentCommunicationViacardInformation(getPaymentCommunicationViacardInformationOutput);
        }

        public async Task<GetPaymentCommunicationViacardPreProcessedDetailResult> GetPaymentCommunicationViacardPreProcessedDetail(string preProcessedId)
        {
            var getPaymentCommunicationViacardPreProcessedDetailOutput = (GetPaymentCommunicationViacardPreProcessedDetailOutput)await serviceManager.Invoke<GetPaymentCommunicationViacardPreProcessedDetailService>(preProcessedId);
            return PaymentServiceResultMapping.GetPaymentCommunicationViacardPreProcessedDetail(getPaymentCommunicationViacardPreProcessedDetailOutput);
        }

        public async Task<ICollection<PreProcessedOperation>> GetPreProcessedOperations(GetPreProcessedOpertationsRequest opertationsRequest)
        {
            var request = PaymentServiceRequestMapping.GetPreProcessedOperations(opertationsRequest);
            var getPreProcessedOpertationsOutput = (GetPreProcessedOpertationsOutput)await serviceManager.Invoke<GetPreProcessedOperationsService>(request);
            return PaymentServiceResultMapping.GetPreProcessedOperations(getPreProcessedOpertationsOutput);
        }

        public async Task<ChallengeData> GetSocialSecurityPayment(GetSocialSecurityPaymentRequest getSocialSecurity)
        {
            PaymentsValidationRequest.GetSocialSecurityPayments(getSocialSecurity);
            var input = PaymentServiceRequestMapping.GetSocialSecurityPayment(getSocialSecurity);
            var output = (ChallengesOutput)await serviceManager.InvokeChallengeService<GetSocialSecurityPaymentService>(input);

            return CommonResultMapping.GetChallengeData(output);
        }

        public async Task<GetSocialSecurityPaymentResult> GetSocialSecurityPaymentConfirmation(ChallengeData challenge)
        {
            var output = (GetSocialSecurityPaymentOutput)await serviceManager.InvokeWithChallenge<GetSocialSecurityPaymentService>(challenge);
            return PaymentServiceResultMapping.GetSocialSecurityPayment(output);
        }

        public async Task<ChallengeData> PaymentCommunicationViaCard(PaymentCommunicationViaCardRequest paymentCommunicationViaCardRequest)
        {
            var request = PaymentServiceRequestMapping.PaymentCommunicationViaCard(paymentCommunicationViaCardRequest);
            var output = (ChallengesOutput)await serviceManager.InvokeChallengeService<PaymentCommunicationViaCardService>(request);

            return CommonResultMapping.GetChallengeData(output);
        }

        public async Task<PaymentCommunicationViaCardResult> PaymentCommunicationViaCard(ChallengeData challengeData)
        {
            var paymentCommunicationViaCardOutput = (PaymentCommunicationViaCardOutput)await serviceManager.InvokeWithChallenge<PaymentCommunicationViaCardService>(challengeData);
            return PaymentServiceResultMapping.PaymentCommunicationViaCard(paymentCommunicationViaCardOutput);
        }

        public async Task<ChallengeData> SocialSecurityPayment(SocialSecurityPaymentRequest request)
        {
            var input = PaymentServiceRequestMapping.SocialSecurityPayment(request);
            var output = (ChallengesOutput)await serviceManager.InvokeChallengeService<SocialSecurityPaymentService>(input);

            return CommonResultMapping.GetChallengeData(output);
        }

        public async Task<string> SocialSecurityPayment(ChallengeData challengeData)
        {
            var output = (SocialSecurityPaymentOutput)await serviceManager.InvokeWithChallenge<SocialSecurityPaymentService>(challengeData);

            return output.PaymentReference;
        }

        public async Task<string> UpdateDebitAuthorization(UpdateDebitAuthorizationRequest request)
        {
            var input = PaymentServiceRequestMapping.UpdateDebitAuthorization(request);
            var output = (UpdateDebitAuthorizationOutput)await serviceManager.Invoke<UpdateDebitAuthorizationService>(input);

            return output.DebitAuthorizationNumber;
        }

        public async Task<ViaverdeAssociationResult> ViaverdeAssociation(ViaverdeAssociationRequest request)
        {
            var input = PaymentServiceRequestMapping.ViaverdeAssociation(request);
            var output = (ViaverdeAssociationOutput)await serviceManager.Invoke<ViaverdeAssociationService>(input);

            return PaymentServiceResultMapping.ViaverdeAssociation(output);
        }

        public async Task<ChallengeData> ServicePayment(ServicePaymentRequest request)
        {
            var input = PaymentServiceRequestMapping.ServicePayment(request);
            var output = (ChallengesOutput)await serviceManager.InvokeChallengeService<ServicePaymentService>(input);

            return CommonResultMapping.GetChallengeData(output);
        }

        public async Task<string> ServicePayment(ChallengeData challengeData)
        {
            var output = (ServicePaymentOutput)await serviceManager.InvokeWithChallenge<ServicePaymentService>(challengeData);

            return output.EntityDescription;
        }

        public async Task<ChallengeData> GetMbwayPaymentInformation(GetMbwayPaymentInformationRequest request)
        {
            var input = PaymentServiceRequestMapping.GetMbwayPaymentInformation(request);
            var output = (ChallengesOutput)await serviceManager.InvokeChallengeService<GetMbwayPaymentInformationService>(input);

            return CommonResultMapping.GetChallengeData(output);
        }

        public async Task<string> GetMbwayPaymentInformation(ChallengeData challengeData)
        {
            var output = (GetMbwayPaymentInformationOutput)await serviceManager.InvokeWithChallenge<GetMbwayPaymentInformationService>(challengeData);

            return output.PaymentReference;
        }

        //$Replace$
    }
}