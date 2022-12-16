using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MMSDemoTrucks.ParametersModel;
using MMSDemoTrucks.HelperQueries;
using Newtonsoft.Json;
using MMSDemoTrucks.Models;

namespace MMSDemoTrucks.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [ApiExplorerSettings(GroupName = "AssignmentAreasTrucks")]
    public class AssignmentAreasTrucks : ControllerBase
    {
        private readonly IConfiguration configuration;
        private readonly string conn;
        private readonly string owner = "CST_00002";
        public AssignmentAreasTrucks(IConfiguration configuration)
        {
            this.configuration = configuration;
            this.conn = this.configuration.GetValue<string>("ConnectionDbSQL:strConn");
        }

        ///// <summary>
        ///// This example shows you how to set the enabled/disabled status
        ///// of your data
        ///// </summary>
        ///// <param name="request"></param>
        ///// <returns></returns>
        //[HttpPost("ControlsEnabled")]
        //public ActionResult<BaseResponse> ControlsEnabled([FromBody] ControlsEnabledRequest request)
        //{
        //    BaseResponse response = new BaseResponse();
        //    try
        //    {
        //        if (request is null)
        //        {
        //            response.Success = false;
        //            response.ErrorMessage = new ErrorMessage("Request is bad formatted");
        //            return new OkObjectResult(response);
        //        }

        //        if (request.ParamIn is null)
        //        {
        //            response.ReturnValue = string.Empty;
        //            response.Success = true;
        //            return new OkObjectResult(response);
        //        }

        //        request.ParamIn.ForEach(obj =>
        //        {
        //            obj.Area.IsReadOnly = true;
        //            obj.AreaDescription.IsReadOnly = true;
        //            obj.TruckCode.IsReadOnly = true;
        //            obj.TruckDescription.IsReadOnly = true;
        //            obj.TruckPlate.IsReadOnly = true;
        //        });

        //        response.ReturnValue = request.ParamIn;
        //        response.Success = true;
        //        return new OkObjectResult(response);
        //    }
        //    catch (Exception e)
        //    {
        //        //throw new Exception("MyBatchBLController ControlsEnabled exception  ", e);
        //        response.Success = false;
        //        response.ErrorMessage = new ErrorMessage(e.Message);
        //        return new OkObjectResult(response);
        //    }
        //}


        /// <summary>
        /// Executes the query to extract data and populate the grid 
        /// with data. After you have queried your db,
        /// the data will be sent back.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("ExtractData")]
        public ActionResult<ExtractDataResponse> ExtractData([FromBody] ExtractDataRequest request)
        {
            ExtractDataResponse response = new ExtractDataResponse();
            string areasFlat;
            List<string> trucksFlat = null;
            List<string> existingAssociations = null;
            Queries queries = new Queries(this.conn);

            try
            {
                //manage your filters here and make a query 
                //that populate the outRows structure
                if (request is null)
                {
                    response.Success = false;
                    response.ErrorMessage = new ErrorMessage("Request is bad formatted");
                    return new OkObjectResult(response);
                }

                //get all areas selected
                if (request.AllAreas)
                    areasFlat = queries.ReadFrom("MA_Areas");
                else
                    areasFlat = queries.ReadFrom("MA_Areas", "Area", request.FromArea, request.ToArea);
                List<SalesAreas> salesAreas = JsonConvert.DeserializeObject<List<SalesAreas>>(areasFlat);

                //get all tucks
                trucksFlat = queries.ReadFromLogical("TrucksMaster", this.owner, true);
                List<TrucksMaster> trucksMaster = new List<TrucksMaster>();
                if (!(trucksFlat is null))
                    trucksFlat.ForEach(t =>
                    {
                        TrucksMaster tAdd = JsonConvert.DeserializeObject<TrucksMaster>(t);
                        if (!(tAdd is null))
                            trucksMaster.Add(tAdd);
                    });

                //get all associations
                existingAssociations = queries.ReadFromLogical("FP_SalesAreaTrucks", this.owner, false, true);
                List<FP_SalesAreaTrucks> associations = new List<FP_SalesAreaTrucks>();
                if (!(existingAssociations is null))
                    existingAssociations.ForEach(ea =>
                    {
                        FP_SalesAreaTrucks eaAdd = JsonConvert.DeserializeObject<FP_SalesAreaTrucks>(ea);
                        if (!(eaAdd is null))
                            associations.Add(eaAdd);
                    });
                
                List<FP_Areas_Trucks> allAssociations = new List<FP_Areas_Trucks>();
                salesAreas.ForEach(sa =>
                {
                    trucksMaster.ForEach(tm =>
                    {
                        FP_Areas_Trucks association = new FP_Areas_Trucks();
                        association.Area = sa.Area;
                        association.AreaDescription = sa.Description;
                        association.TruckCode = tm.Code;
                        association.TruckDescription = tm.Description;
                        association.TruckPlate = tm.Plate;
                        association.Selected = false;
                        FP_SalesAreaTrucks foundAssociation = associations.Find(a => a.Area.CompareTo(sa.Area) == 0 && a.TruckCode.CompareTo(tm.Code) == 0);
                        if (!(foundAssociation is null))
                            association.Selected = true;
                        
                        allAssociations.Add(association);
                    });
                    
                });

                //set returnValue on response
                response.ReturnValue = allAssociations;
                return new OkObjectResult(response);
            }
            catch (Exception e)
            {
                //throw new Exception("AssignmentAreasTrucks ExtractData exception  ", e);
                response.Success = false;
                response.ErrorMessage = new ErrorMessage("Cannot extract data");
                return new OkObjectResult(response);
            }
        }

        /// <summary>
        /// Populate the grid 
        /// with the elaborated data. After you have queried your db,
        /// the data will be sent back.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("BatchExecute")]
        public ActionResult<BatchExecuteResponse> BatchExecute([FromBody] BatchExecuteRequest request)
        {
            List<FP_Areas_Trucks> associationsSaved = new List<FP_Areas_Trucks>();
            BatchExecuteResponse response = new BatchExecuteResponse();
            //List<string> existingAssociations = null;
            Queries queries = new Queries(this.conn);
            try
            {
                
                if (request is null || request.ParamIn is null)
                {
                    response.Success = false;
                    response.ErrorMessage = new ErrorMessage("Request is bad formatted");
                    return new OkObjectResult(response);
                }

                //clear all existing associations
                if (!queries.ClearFromLogical("FP_SalesAreaTrucks", this.owner, false, true))
                {
                    response.Success = false;
                    response.ErrorMessage = new ErrorMessage("Cannot update logical table fp_SalesAreaTrucks");
                    return new OkObjectResult(response);
                }

                if (!(request.ParamIn is null))
                {
                    request.ParamIn.ForEach(at =>
                    {
                        if (at.Selected)
                        {
                            FP_SalesAreaTrucks aToSave = new FP_SalesAreaTrucks();
                            aToSave.Area = at.Area;
                            aToSave.TruckCode = at.TruckCode;
                            string jsonContent = JsonConvert.SerializeObject(aToSave);
                            string keys = $"{aToSave.Area}##{aToSave.TruckCode}";
                            string parentKeys = aToSave.Area;
                            bool success = queries.SaveEntityIntoLogicalTable("FP_SalesAreaTrucks", this.owner, jsonContent,
                                keys, parentKeys, false, true);
                            if (success)
                                associationsSaved.Add(at);
                        }
                    });

                }
                
                response.ReturnValue = associationsSaved;
                response.Success = true;
                return new OkObjectResult(response);
            }
            catch (Exception e)
            {
                //throw new Exception("MyBatchBLController BatchExecute exception  ", e);
                response.Success = false;
                response.ErrorMessage = new ErrorMessage("Cannot update logical table fp_SalesAreaTrucks");
                return new OkObjectResult(response);
            }
        }
    } 
}
