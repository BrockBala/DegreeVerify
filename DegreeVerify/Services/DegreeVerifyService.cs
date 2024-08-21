using Dapper;
using DegreeVerify.Client.Services.IServices;
using DegreeVerify.DTO;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace DegreeVerify.Client.Services
{
    public class DegreeVerifyService
    {
        private readonly HttpClient _httpClient;
        private readonly IAppSettings _appsSetting;
        private readonly TokenService _tokenService;
        public DegreeVerifyService(AppSettings appsSetting, TokenService tokenService, HttpClient httpClient)
        {
            _appsSetting = appsSetting;
            _tokenService = tokenService;
            _httpClient = httpClient;
        }

        public async Task DegreeVerify()
        {

            await PostVerify(1, _appsSetting["DegreeVerifyEndPoints:DegreeVerify"]);
        }

        public async Task DOAVerify()
        {
            await PostVerify(2, _appsSetting["DegreeVerifyEndPoints:DOAVerify"]);
        }

        public async Task DegreeHistory()
        {
            await History(_appsSetting["DegreeVerifyEndPoints:DegreeHistory"]);
        }

        public async Task DOAHistory()
        {
            await History(_appsSetting["DegreeVerifyEndPoints:DOAHistory"]);
        }

        public async Task Cancel()
        {
            try
            {
                var token = await _tokenService.GetAccessToken();

                if (token != null)
                {
                    var uri = _appsSetting["DegreeVerifyEndPoints:Cancel"];
                    HistoryRequestDTO request = new HistoryRequestDTO
                    {
                        AccountId = "10041631",
                        TransactionId = "100000700"
                    };
                    uri = $"{uri}?accountId={request.AccountId}&transactionId={request.TransactionId}";
                    _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(token.TokenType, token.AccessToken);
                    var response = await _httpClient.PostAsync(uri, null);
                }
            }
            catch (Exception ex)
            {

            }
        }

        private async Task PostVerify(byte requesttypeId, string uri)
        {
            try
            {
                var token = await _tokenService.GetAccessToken();

                if (token != null)
                {
                    var degreeVerifyRequest = new DegreeVerifyRequestDTO
                    {
                        AccountId = "10041631",
                        OrganizationName = "ABC Company",
                        CaseReferenceId = "ABC123",
                        ContactEmail = "AbcCompany@email.com",
                        SSN = "123456401",
                        DateOfBirth = "1990-04-01",
                        FirstName = "John",
                        LastName = "Smith",
                        Terms = "Y",
                        EndClient = "DEF Company",
                        SchoolCode = "003754",
                        PreviousNames = {
                    new PreviousName{FirstName = "Req_FirsrName1", MiddleName = "Req_MiddleName1", LastName = "Req_LastName1"},
                    new PreviousName{FirstName = "Req_FirsrName2", MiddleName = "Req_MiddleName2", LastName = "Req_LastName2"}
                    }
                    };
                    degreeVerifyRequest.RequestTypeId = requesttypeId;
                    _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(token.TokenType, token.AccessToken);
                    var response = await _httpClient.PostAsJsonAsync(uri, degreeVerifyRequest);
                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        var responseString = await response.Content.ReadAsStringAsync();
                        var degree = JsonConvert.DeserializeObject<DegreeDTO>(responseString);
                        await UpdateDegreeVerifiedData(degree, degreeVerifyRequest);
                    }
                }
            }
            catch (Exception ex)
            {

            }
        }

        private async Task History(string uri)
        {
            try
            {
                var token = await _tokenService.GetAccessToken();

                if (token != null)
                {
                    HistoryRequestDTO request = new HistoryRequestDTO
                    {
                        AccountId = "10041631",
                        TransactionId = "100000408"
                    };
                    _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(token.TokenType, token.AccessToken);
                    uri = $"{uri}?accountId={request.AccountId}&transactionId={request.TransactionId}";
                    var response = await _httpClient.GetAsync(uri);

                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        var responseString = await response.Content.ReadAsStringAsync();
                        var degree = JsonConvert.DeserializeObject<DegreeDTO>(responseString);
                        await UpdateDegreeVerifiedData(degree, null);
                    }
                }
            }
            catch (Exception ex)
            {

            }
        }

        private async Task UpdateDegreeVerifiedData(DegreeDTO degree, DegreeVerifyRequestDTO requestBody)
        {
            string connectionString = _appsSetting["ConnectionStrings:DefaultConnection"];
            var con = new SqlConnection(connectionString);

            var parameters = new DynamicParameters();

            var previousNamesTable = PreviousNamesTable();
            var degreeVerifyRequestTable = DegreeVerifyRequestTable(requestBody, previousNamesTable);
            var statusTable = StatusTable(degree.Status);
            var transactionTable = TransactionTable(degree.TransactionDetails);
            var clientDataTable = ClientDataTable(degree.ClientData);
            var studentInfoTable = StudentInfoTable(degree.StudentInfoProvided, previousNamesTable);
            var infoProvidedBySchoolTable = InfoProvidedBySchoolTable(degree.InfoProvidedBySchool);

            var degreeDetailsTable = DegreeDetailsTable();
            var enrollmentDetailTable = EnrollmentDetailTable();
            var enrollmentDataTable = EnrollmentDataTable();
            var majorCoursesTable = CoursesOfStudyTable();
            var minorCoursesTable = CoursesOfStudyTable();
            var majorOptionsTable = CoursesTable();
            var majorConcentrationsTable = CoursesTable();


            int refId = 1;
            foreach (var degreeDetail in degree.DegreeDetails)
            {
                degreeDetailsTable.Rows.Add(degreeDetail.DegreeId, degreeDetail.NameOnSchoolRecord?.FirstName, degreeDetail.NameOnSchoolRecord?.MiddleName, degreeDetail.NameOnSchoolRecord?.LastName, degreeDetail.NameOnSchoolRecord?.NameSuffix, degreeDetail.DegreeStatus, degreeDetail.OfficialSchoolName, degreeDetail.SchoolCode, degreeDetail.BranchCode, degreeDetail.SchoolDivision, degreeDetail.JointInstitution, degreeDetail.DegreeTitle, degreeDetail.AwardDate, degreeDetail.AcademicHonors, degreeDetail.DatesOfAttendance?.DatesOfAttendanceId, degreeDetail.DatesOfAttendance?.TermBeginDate, degreeDetail.DatesOfAttendance?.TermEndDate, degreeDetail.HonorsProgram, degreeDetail.OtherHonors, refId);

                foreach (var course in degreeDetail.MajorCoursesOfStudy)
                {
                    majorCoursesTable.Rows.Add(course.CourseName, course.NcesCIPCode, refId);
                }

                foreach (var course in degreeDetail.MinorCoursesOfStudy)
                {
                    minorCoursesTable.Rows.Add(course.CourseName, course.NcesCIPCode, refId);
                }

                foreach (var course in degreeDetail.MajorOptions)
                {
                    majorOptionsTable.Rows.Add(course.CourseName, refId);
                }

                foreach (var course in degreeDetail.MajorConcentrations)
                {
                    majorConcentrationsTable.Rows.Add(course.CourseName, refId);
                }
                refId++;
            }

            var enrollDataRefId = 1;
            foreach (var enrollmentDetail in degree.EnrollmentDetails)
            {
                enrollmentDetailTable.Rows.Add(enrollmentDetail.NameOnSchoolRecord?.FirstName, enrollmentDetail.NameOnSchoolRecord?.MiddleName, enrollmentDetail.NameOnSchoolRecord?.LastName, enrollmentDetail.NameOnSchoolRecord?.NameSuffix, enrollmentDetail.OfficialSchoolName, enrollmentDetail.CurrentEnrollmentStatus, enrollmentDetail.EnrollmentSinceDate, enrollmentDetail.SchoolCode, enrollmentDetail.BranchCode, enrollDataRefId);

                foreach (var enrollmentData in enrollmentDetail.EnrollmentData)
                {
                    enrollmentDataTable.Rows.Add(enrollmentData.EnrollmentId, enrollmentData.EnrollmentStatus, enrollmentData.TermBeginDate, enrollmentData.TermEndDate, enrollmentData.SchoolCertifiedOnDate, enrollmentData.AnticipatedGraduationDate, enrollDataRefId, refId);

                    foreach (var course in enrollmentData.MajorCoursesOfStudy)
                    {
                        majorCoursesTable.Rows.Add(course.CourseName, course.NcesCIPCode, refId);
                    }
                    refId++;
                }
                enrollDataRefId++;
            }

            parameters.Add("@DegreeVerifyRequest", degreeVerifyRequestTable.AsTableValuedParameter("dbo.DegreeVerifyRequest_Type"));
            parameters.Add("@PreviousNames", previousNamesTable.AsTableValuedParameter("dbo.PreviousNames_Type"));
            parameters.Add("@Status", statusTable.AsTableValuedParameter("dbo.Status_Type"));
            parameters.Add("@TransactionDetails", transactionTable.AsTableValuedParameter("dbo.TransactionDetails_Type"));
            parameters.Add("@ClientData", clientDataTable.AsTableValuedParameter("dbo.ClientData_Type"));
            parameters.Add("@StudentInfo", studentInfoTable.AsTableValuedParameter("dbo.StudentInfo_Type"));
            parameters.Add("@DegreeDetails", degreeDetailsTable.AsTableValuedParameter("dbo.DegreeDetails_Type"));
            parameters.Add("@EnrollmentDetails", enrollmentDetailTable.AsTableValuedParameter("dbo.EnrollmentDetails_Type"));
            parameters.Add("@EnrollmentDatas", enrollmentDataTable.AsTableValuedParameter("dbo.EnrollmentData_Type"));
            parameters.Add("@InfoProvidedBySchool", infoProvidedBySchoolTable.AsTableValuedParameter("dbo.InfoProvidedBySchool_Type"));
            parameters.Add("@MajorCourses", majorCoursesTable.AsTableValuedParameter("dbo.CourseOfStudy_Type"));
            parameters.Add("@MinorCourses", minorCoursesTable.AsTableValuedParameter("dbo.CourseOfStudy_Type"));
            parameters.Add("@MajorOptions", majorOptionsTable.AsTableValuedParameter("dbo.Course_Type"));
            parameters.Add("@MajorConcentrations", majorConcentrationsTable.AsTableValuedParameter("dbo.Course_Type"));

            await con.ExecuteAsync("UpdateDegreeVerifiedData", parameters, commandType: CommandType.StoredProcedure);
        }

        private DataTable DegreeVerifyRequestTable(DegreeVerifyRequestDTO requestBody, DataTable previousNamesTable)
        {
            DataTable table = new DataTable();
            table.Columns.Add("RequestTypeId", typeof(int));
            table.Columns.Add("AccountId", typeof(string));
            table.Columns.Add("OrganizationName", typeof(string));
            table.Columns.Add("CaseReferenceId", typeof(string));
            table.Columns.Add("CorrelationId", typeof(string));
            table.Columns.Add("ContactEmail", typeof(string));
            table.Columns.Add("SSN", typeof(string));
            table.Columns.Add("DateOfBirth", typeof(DateTime));
            table.Columns.Add("LastName", typeof(string));
            table.Columns.Add("FirstName", typeof(string));
            table.Columns.Add("MiddleName", typeof(string));
            table.Columns.Add("StudentId", typeof(string));
            table.Columns.Add("Terms", typeof(string));
            table.Columns.Add("SchoolCode", typeof(string));
            table.Columns.Add("BranchCode", typeof(string));
            table.Columns.Add("DegreeTitle", typeof(string));
            table.Columns.Add("YearAwarded", typeof(int));
            table.Columns.Add("Major", typeof(string));
            table.Columns.Add("DegreeLevelCode", typeof(string));
            table.Columns.Add("ApplyLikeSchoolMatching", typeof(string));
            table.Columns.Add("Email", typeof(string));
            table.Columns.Add("Phone", typeof(string));
            table.Columns.Add("Address1", typeof(string));
            table.Columns.Add("Address2", typeof(string));
            table.Columns.Add("City", typeof(string));
            table.Columns.Add("State", typeof(string));
            table.Columns.Add("ZipCode", typeof(string));
            table.Columns.Add("EndClient", typeof(string));
            table.Columns.Add("SecondaryClient", typeof(string));
            table.Columns.Add("StartDate", typeof(DateTime));
            table.Columns.Add("JobTitle", typeof(string));
            table.Columns.Add("NaicsCode", typeof(string));
            if (requestBody != null)
            {
                table.Rows.Add(
                2,
                requestBody.AccountId,
                requestBody.OrganizationName,
                requestBody.CaseReferenceId,
                requestBody.CorrelationId,
                requestBody.ContactEmail,
                requestBody.SSN,
                requestBody.DateOfBirth,
                requestBody.LastName,
                requestBody.FirstName,
                requestBody.MiddleName,
                requestBody.StudentId,
                requestBody.Terms,
                requestBody.SchoolCode,
                requestBody.BranchCode,
                requestBody.DegreeTitle,
                requestBody.YearAwarded,
                requestBody.Major,
                requestBody.DegreeLevelCode,
                requestBody.ApplyLikeSchoolMatching,
                requestBody.Email,
                requestBody.Phone,
                requestBody.Address1,
                requestBody.Address2,
                requestBody.City,
                requestBody.State,
                requestBody.ZipCode,
                requestBody.EndClient,
                requestBody.SecondaryClient,
                requestBody.StartDate,
                requestBody.JobTitle,
                requestBody.NaicsCode
                );

                foreach (var previousName in requestBody.PreviousNames)
                {
                    previousNamesTable.Rows.Add(previousName.FirstName, previousName.MiddleName, previousName.LastName, false);
                }
            }
            return table;
        }

        private DataTable StatusTable(Status status)
        {
            DataTable table = new DataTable();
            table.Columns.Add("Code", typeof(string));
            table.Columns.Add("Severity", typeof(string));
            table.Columns.Add("Message", typeof(string));

            if (status != null)
            {
                table.Rows.Add(status.Code, status.Severity, status.Message);
            }
            return table;
        }

        private DataTable TransactionTable(TransactionDetails trans)
        {
            DataTable table = new DataTable();
            table.Columns.Add("TransactionId", typeof(string));
            table.Columns.Add("OrderId", typeof(string));
            table.Columns.Add("TransactionStatus", typeof(string));
            table.Columns.Add("TransactionFee", typeof(decimal));
            table.Columns.Add("SalesTax", typeof(decimal));
            table.Columns.Add("TransactionTotal", typeof(decimal));
            table.Columns.Add("RequestedBy", typeof(string));
            table.Columns.Add("RequestedDate", typeof(DateTime));
            table.Columns.Add("NotifiedDate", typeof(DateTime));
            table.Columns.Add("NscHit", typeof(string));
            table.Columns.Add("SchoolContactHistory", typeof(string));
            table.Columns.Add("AppliedLikeSchool", typeof(string));
            table.Columns.Add("StudentComments", typeof(string));

            if (trans != null)
            {
                table.Rows.Add(trans.TransactionId, trans.OrderId, trans.TransactionStatus, trans.TransactionFee, trans.SalesTax, trans.TransactionTotal, trans.RequestedBy, trans.RequestedDate, trans.NotifiedDate, trans.NscHit, trans.SchoolContactHistory, trans.AppliedLikeSchool, trans.StudentComments);
            }
            return table;
        }

        private DataTable ClientDataTable(ClientData client)
        {
            DataTable table = new DataTable();
            table.Columns.Add("AccountId", typeof(string));
            table.Columns.Add("ContactEmail", typeof(string));
            table.Columns.Add("CorrelationId", typeof(string));
            table.Columns.Add("OrganizationName", typeof(string));
            table.Columns.Add("CaseReferenceId", typeof(string));

            if (client != null)
            {
                table.Rows.Add(
                    client.AccountId,
                    client.ContactEmail,
                    client.CorrelationId,
                    client.OrganizationName,
                    client.CaseReferenceId
                    );
            }
            return table;
        }

        private DataTable StudentInfoTable(StudentInfoProvided info, DataTable previousNamesTable)
        {
            DataTable table = new DataTable();
            table.Columns.Add("FirstName", typeof(string));
            table.Columns.Add("MiddleName", typeof(string));
            table.Columns.Add("LastName", typeof(string));
            table.Columns.Add("DateOfBirth", typeof(DateTime));
            table.Columns.Add("SchoolCode", typeof(string));
            table.Columns.Add("BranchCode", typeof(string));
            table.Columns.Add("DegreeLevel", typeof(string));
            table.Columns.Add("ApplyLikeSchoolMatching", typeof(string));
            table.Columns.Add("DegreeTitle", typeof(string));
            table.Columns.Add("YearAwarded", typeof(int));
            table.Columns.Add("Major", typeof(string));

            if (info != null)
            {
                table.Rows.Add(
                    info.FirstName,
                    info.MiddleName,
                    info.LastName,
                    info.DateOfBirth,
                    info.SchoolCode,
                    info.BranchCode,
                    info.DegreeLevel,
                    info.ApplyLikeSchoolMatching,
                    info.DegreeTitle,
                    info.YearAwarded,
                    info.Major
                );

                foreach (var previousName in info.PreviousNames)
                {
                    previousNamesTable.Rows.Add(previousName.FirstName, previousName.MiddleName, previousName.LastName, true);
                }
            }
            return table;
        }

        private DataTable InfoProvidedBySchoolTable(List<InfoProvidedBySchool> infos)
        {
            var table = new DataTable();
            table.Columns.Add("ProjectedGradDate", typeof(DateTime));
            table.Columns.Add("SchoolComment", typeof(string));
            table.Columns.Add("UpdatedDate", typeof(DateTime));

            foreach (var info in infos)
            {
                table.Rows.Add(info.ProjectedGradDate, info.SchoolComment, info.UpdatedDate);
            }

            return table;
        }

        private DataTable DegreeDetailsTable()
        {
            var table = new DataTable();
            table.Columns.Add("DegreeId", typeof(string));
            table.Columns.Add("SchoolRecordFirstName", typeof(string));
            table.Columns.Add("SchoolRecordMiddleName", typeof(string));
            table.Columns.Add("SchoolRecordLastName", typeof(string));
            table.Columns.Add("SchoolRecordNameSuffix", typeof(string));
            table.Columns.Add("DegreeStatus", typeof(string));
            table.Columns.Add("OfficialSchoolName", typeof(string));
            table.Columns.Add("SchoolCode", typeof(string));
            table.Columns.Add("BranchCode", typeof(string));
            table.Columns.Add("SchoolDivision", typeof(string));
            table.Columns.Add("JointInstitution", typeof(string));
            table.Columns.Add("DegreeTitle", typeof(string));
            table.Columns.Add("AwardDate", typeof(DateTime));
            table.Columns.Add("AcademicHonors", typeof(string));
            table.Columns.Add("DatesOfAttendanceId", typeof(int));
            table.Columns.Add("TermBeginDate", typeof(DateTime));
            table.Columns.Add("TermEndDate", typeof(DateTime));
            table.Columns.Add("HonorsProgram", typeof(string));
            table.Columns.Add("OtherHonors", typeof(string));
            table.Columns.Add("RefId", typeof(int));

            return table;
        }

        private DataTable EnrollmentDetailTable()
        {
            var table = new DataTable();
            table.Columns.Add("SchoolRecordFirstName", typeof(string));
            table.Columns.Add("SchoolRecordMiddleName", typeof(string));
            table.Columns.Add("SchoolRecordLastName", typeof(string));
            table.Columns.Add("SchoolRecordNameSuffix", typeof(string));
            table.Columns.Add("OfficialSchoolName", typeof(string));
            table.Columns.Add("CurrentEnrollmentStatus", typeof(string));
            table.Columns.Add("EnrollmentSinceDate", typeof(DateTime));
            table.Columns.Add("SchoolCode", typeof(string));
            table.Columns.Add("BranchCode", typeof(string));
            table.Columns.Add("RefId", typeof(int));

            return table;
        }

        private DataTable EnrollmentDataTable()
        {
            var table = new DataTable();
            table.Columns.Add("EnrollmentId", typeof(string));
            table.Columns.Add("EnrollmentStatus", typeof(string));
            table.Columns.Add("TermBeginDate", typeof(DateTime));
            table.Columns.Add("TermEndDate", typeof(DateTime));
            table.Columns.Add("SchoolCertifiedOnDate", typeof(DateTime));
            table.Columns.Add("AnticipatedGraduationDate", typeof(DateTime));
            table.Columns.Add("DetailRefId", typeof(int));
            table.Columns.Add("RefId", typeof(int));

            return table;
        }

        private DataTable CoursesOfStudyTable()
        {
            var table = new DataTable();
            table.Columns.Add("Course", typeof(string));
            table.Columns.Add("NcesCIPCode", typeof(string));
            table.Columns.Add("RefId", typeof(int));

            return table;
        }

        private DataTable CoursesTable()
        {
            var table = new DataTable();
            table.Columns.Add("Course", typeof(string));
            table.Columns.Add("RefId", typeof(int));

            return table;
        }

        private DataTable PreviousNamesTable()
        {
            var table = new DataTable();
            table.Columns.Add("FirstName", typeof(string));
            table.Columns.Add("MiddleName", typeof(string));
            table.Columns.Add("LastName", typeof(string));
            table.Columns.Add("IsResponse", typeof(bool));
            return table;
        }

    }
}
