using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using University.Interfaces;
using University.Services;

namespace University.Tests;

[TestClass]
public class ValidationTest
{
    private IValidationService validationService;

    [TestInitialize()]
    public void Initialize()
    {
        validationService = new ValidationService();
    }

    [TestMethod]
    public void ValidateNullDate()
    {
        Assert.IsFalse(validationService.ValidateBirthDate(null));
    }   

    [TestMethod]
    public void ValidateFutureDate()
    {
        Assert.IsFalse(validationService.ValidateBirthDate(DateTime.Now.AddDays(1)));
    }  

    [TestMethod]
    public void ValidateCorrectDate()
    {
        Assert.IsTrue(validationService.ValidateBirthDate(DateTime.Now.AddYears(-1)));
    }        
    
    [TestMethod]
    public void ValidateCorrectPESEL()
    {
        Assert.IsTrue(validationService.ValidatePESEL("88012471472"));
    }        

    [TestMethod]
    public void ValidateIncorrectPESEL()
    {
        Assert.IsFalse(validationService.ValidatePESEL("88012471471"));
    }
}
