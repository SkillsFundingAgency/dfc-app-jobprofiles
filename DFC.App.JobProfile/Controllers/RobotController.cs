using DFC.App.JobProfile.Models.Robots;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Net.Mime;

namespace DFC.App.JobProfile.Controllers
{
    public class RobotController : Controller
    {
        private readonly ILogger<RobotController> _logger;
        private readonly IWebHostEnvironment _environment;

        public RobotController(
            ILogger<RobotController> logger,
            IWebHostEnvironment environment)
        {
            _logger = logger;
            _environment = environment;
        }

        [HttpGet]
        public ContentResult Robot()
        {
            try
            {
                _logger.LogInformation("Generating Robots.txt");

                var robot = GenerateThisSiteRobot();

                _logger.LogInformation("Generated Robots.txt");

                return Content(robot.Data, MediaTypeNames.Text.Plain);
            }
            catch (Exception ex)
            {
                _logger.LogError($"{nameof(Robot)}: {ex.Message}");
            }

            // fall through from errors
            return Content(null, MediaTypeNames.Text.Plain);
        }

        private Robot GenerateThisSiteRobot()
        {
            var robot = new Robot();
            var robotsFilePath = Path.Combine(_environment.WebRootPath, "StaticRobots.txt");

            if (System.IO.File.Exists(robotsFilePath))
            {
                // output the composite UI default (static) robots data from the StaticRobots.txt file
                var staticRobotsText = System.IO.File.ReadAllText(robotsFilePath);

                if (!string.IsNullOrWhiteSpace(staticRobotsText))
                {
                    robot.Add(staticRobotsText);
                }
            }

            return robot;
        }
    }
}