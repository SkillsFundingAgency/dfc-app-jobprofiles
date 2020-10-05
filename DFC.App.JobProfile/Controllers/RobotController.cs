using DFC.App.JobProfile.Models.Robots;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Net.Mime;

namespace DFC.App.JobProfile.Controllers
{
    public class RobotController : Controller
    {
        private readonly ILogger<RobotController> logService;
        private readonly IHostingEnvironment hostingEnvironment;

        public RobotController(ILogger<RobotController> logService, IHostingEnvironment hostingEnvironment)
        {
            this.logService = logService;
            this.hostingEnvironment = hostingEnvironment;
        }

        [HttpGet]
        public ContentResult Robot()
        {
            try
            {
                logService.LogInformation("Generating Robots.txt");

                var robot = GenerateThisSiteRobot();

                logService.LogInformation("Generated Robots.txt");

                return Content(robot.Data, MediaTypeNames.Text.Plain);
            }
            catch (Exception ex)
            {
                logService.LogError($"{nameof(Robot)}: {ex.Message}");
            }

            // fall through from errors
            return Content(null, MediaTypeNames.Text.Plain);
        }

        private Robot GenerateThisSiteRobot()
        {
            var robot = new Robot();
            string robotsFilePath = System.IO.Path.Combine(hostingEnvironment.WebRootPath, "StaticRobots.txt");

            if (System.IO.File.Exists(robotsFilePath))
            {
                // output the composite UI default (static) robots data from the StaticRobots.txt file
                string staticRobotsText = System.IO.File.ReadAllText(robotsFilePath);

                if (!string.IsNullOrWhiteSpace(staticRobotsText))
                {
                    robot.Add(staticRobotsText);
                }
            }

            return robot;
        }
    }
}