﻿using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Campervibe.IntegrationTests.Common;
using Campervibe.Domain.Entities;
using Campervibe.Domain.Requests;
using Campervibe.Domain.RepositoryContracts;
using Campervibe.Domain.Constants;
using Campervibe.IntegrationTest.Common;
using Ninject;
using Ninject.Activation;
using Campervibe.Data.Common;

namespace Campervibe.IntegrationTest.Repositories
{
    [TestClass]
    public class BookingRepositoryTest
    {
        private IContextProvider _contextProvider;
        private IBookingRepository _bookingRepository;
        private IUserRepository _userRepository;
        private IVehicleRepository _vehicleRepository;

        [TestInitialize]
        public void SetUp()
        {
            _contextProvider = TestRegistry.Kernel.Get<IContextProvider>();
            _bookingRepository = TestRegistry.Kernel.Get<IBookingRepository>();
            _userRepository = TestRegistry.Kernel.Get<IUserRepository>();
            _vehicleRepository = TestRegistry.Kernel.Get<IVehicleRepository>();
            ScriptRunner.RunScript();
        }

        [TestMethod]
        public void CanAddBooking()
        {
            Guid bookingId;

            var makeBookingRequest = new MakeBookingRequest()
            {
                StartDate = new DateTime(2015, 01, 12),
                EndDate = new DateTime(2015, 01, 16)
            };

            using (_contextProvider)
            {
                var applicationUser = _userRepository.GetById(UserIds.Application);
                makeBookingRequest.Vehicle = _vehicleRepository.GetById(VehicleIds.SF59QRT);

                makeBookingRequest.Customer = Customer.Register(new RegisterCustomerRequest()
                {
                    UserId = Guid.NewGuid(),
                    EmailAddress = "gary@green.com",
                    //CustomerRole = 
                    FamilyName = "Green",
                    GivenName = "Gary",
                    Address1 = "3 Green Road",
                    Address2 = "Greenton",
                    Address3 = "Greenshire",
                    PostCode = "GN1 1AA",
                    ApplicationUser = applicationUser
                });

                var booking = Booking.Make(makeBookingRequest);
                _bookingRepository.Save(booking);
                bookingId = booking.Id.Value;
                _contextProvider.SaveChanges();
            }

            using (_contextProvider)
            {
                var allBookings = _bookingRepository.GetAll();
                var storedBooking = _bookingRepository.GetById(bookingId);
                Assert.AreEqual(1, allBookings.Count);
                Assert.IsNotNull(storedBooking.Id);
                Assert.IsNotNull(storedBooking.BookingNumber);
                Assert.AreEqual(makeBookingRequest.EndDate, storedBooking.EndDate);
                Assert.AreEqual(makeBookingRequest.EndDate, storedBooking.EndDate);
                Assert.AreEqual(makeBookingRequest.Customer.User.Id.Value, storedBooking.CreatedBy.Id.Value);
                Assert.AreEqual("Green", storedBooking.Customer.FamilyName);
                Assert.AreEqual("Gary", storedBooking.Customer.GivenName);
                Assert.AreEqual("Ford", storedBooking.Vehicle.Make);
                Assert.AreEqual("Transit", storedBooking.Vehicle.Model);
            }
        }
    }
}
