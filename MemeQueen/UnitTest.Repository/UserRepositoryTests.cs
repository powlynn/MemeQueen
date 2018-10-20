﻿using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Repository.DataModels;
using Repository.Repositories;

namespace UnitTests.Repository
{
    [TestClass]
    public class CustomMessageRepositoryTests
    {
        UserRepository repository;

        [TestInitialize]
        public void Init()
        {
            repository = new UserRepository();
        }

        //USERS
        [TestMethod]
        public void TestAddUser()
        {
            var user = new User()
            {
                FirstName = "Joe",
                LastName = "Poulin",
                PhoneNumber = "+17152205642",
                EmailAddress = "josephdpoulin@gmail.com",
                UsePhoneNumber = false,
                UseEmailAddress = true,
                MemeTime = new DateTime(2000, 1, 1, 9, 30, 0)
            };

            var result = repository.AddUser(user);

            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void TestGetUsers()
        {
            var result = repository.GetUsers();

            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void TestGetUserById()
        {
            var result = repository.GetUserById(1);

            Assert.IsNotNull(result);
        }

        //CUSTOM MESSAGES
        [TestMethod]
        public void TestAddCustomMessage()
        {
            var user = repository.GetUserById(1);

            var message = new CustomMessage()
            {
                MessageText = "I'm a test",
                User = user,
                Date = new DateTime(2018, 10, 9)
            };

            var result = repository.AddCustomMessage(message);

            Assert.IsNotNull(result);
        }
    }
}
