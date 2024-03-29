﻿using MedicalClinicWebApi.BLLDTOs;
using MedicalClinicWebApi.BLL.Interfaces;
using MedicalClinicWebApi.Common.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace MedicalClinicWebApi.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LabOrdersController : ControllerBase
    {
        private readonly ILabOrdersLogic _labOrdersLogic;
        private readonly IUserService _userService;

        public LabOrdersController(ILabOrdersLogic labOrdersLogic, IUserService userService)
        {
            _labOrdersLogic = labOrdersLogic;
            _userService = userService;
        }

        [HttpGet]
        public async Task<IActionResult> Get(string patientId)
        {
            var labOrders = await _labOrdersLogic.GetAllLabOrders(patientId);
            return labOrders != null ? Ok(labOrders) : NotFound();
        }

        [HttpGet("{id}/{patientid}")]
        public async Task<IActionResult> Get(int id, string patientId)
        {
            var labOrder = await _labOrdersLogic.GetLabOrderByID(id, patientId);
            return labOrder != null ? Ok(labOrder) : NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] LabOrderDto labOrder)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Record object is invalid!");
            }

            var patientExists = await _userService.FindUserByID(labOrder.PatientId);

            if (patientExists == null)
            {
                return NotFound("That patient with the supplied patientId could not be found in the database!");
            }

            var returnedOrder = await _labOrdersLogic.CreateLabOrder(labOrder);

            return Created("", returnedOrder);
        }

        [HttpPut]
        public async Task<IActionResult> Put([FromBody] LabOrderDto labOrder)
        {
            try
            {
                await _labOrdersLogic.UpdateLabOrder(labOrder);
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { Status = "Error", e.Message });
            }

            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            await _labOrdersLogic.DeleteLabOrder(id);

            return Ok();
        }
    }
}