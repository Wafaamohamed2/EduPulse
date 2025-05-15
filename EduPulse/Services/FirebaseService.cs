using FirebaseAdmin;
using FirebaseAdmin.Messaging;
using Google.Apis.Auth.OAuth2;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace EduPulse.Services
{
    public class FirebaseService
    {
        private readonly FirebaseMessaging? _messaging;
        private readonly IConfiguration _configuration;
        private readonly bool _isInitialized;

        public FirebaseService(IConfiguration configuration)
        {
            _configuration = configuration;
            _isInitialized = false;

            try
            {
                // Check if notifications are enabled in configuration
                bool notificationsEnabled = _configuration.GetValue<bool>("Firebase:NotificationsEnabled", false);
                if (!notificationsEnabled)
                {
                    Console.WriteLine("Firebase notifications are disabled in configuration.");
                    return;
                }

                // First check if the path is configured in settings
                string? configuredPath = _configuration["Firebase:CredentialsPath"];
                string credentialPath = "";
                bool fileFound = false;

                if (!string.IsNullOrEmpty(configuredPath))
                {
                    // Try relative to current directory
                    var relativePath = Path.Combine(Directory.GetCurrentDirectory(), configuredPath);
                    if (File.Exists(relativePath))
                    {
                        credentialPath = relativePath;
                        fileFound = true;
                        Console.WriteLine($"Found Firebase credentials at configured relative path: {relativePath}");
                    }
                    // Try absolute path
                    else if (File.Exists(configuredPath))
                    {
                        credentialPath = configuredPath;
                        fileFound = true;
                        Console.WriteLine($"Found Firebase credentials at configured absolute path: {configuredPath}");
                    }
                }
                
                if (!fileFound)
                {
                    // Fallback to default locations
                    var baseDirectoryPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Config", "edupulse-firebase-adminsdk.json");
                    if (File.Exists(baseDirectoryPath))
                    {
                        credentialPath = baseDirectoryPath;
                        fileFound = true;
                        Console.WriteLine($"Found Firebase credentials at base directory path: {baseDirectoryPath}");
                    }
                    else
                    {
                        var currentDirectoryPath = Path.Combine(Directory.GetCurrentDirectory(), "Config", "edupulse-firebase-adminsdk.json");
                        if (File.Exists(currentDirectoryPath))
                        {
                            credentialPath = currentDirectoryPath;
                            fileFound = true;
                            Console.WriteLine($"Found Firebase credentials at current directory path: {currentDirectoryPath}");
                        }
                    }
                }

                if (!fileFound)
                {
                    Console.WriteLine("Firebase credentials file not found. Notifications will be disabled.");
                    Console.WriteLine($"Searched paths:");
                    Console.WriteLine($"- {Path.Combine(Directory.GetCurrentDirectory(), configuredPath ?? "")}");
                    Console.WriteLine($"- {configuredPath ?? "Not configured"}");
                    Console.WriteLine($"- {Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Config", "edupulse-firebase-adminsdk.json")}");
                    Console.WriteLine($"- {Path.Combine(Directory.GetCurrentDirectory(), "Config", "edupulse-firebase-adminsdk.json")}");
                    return;
                }

                // Initialize Firebase Admin SDK if not already initialized
                if (FirebaseApp.DefaultInstance == null)
                {
                    FirebaseApp.Create(new AppOptions
                    {
                        Credential = GoogleCredential.FromFile(credentialPath)
                    });
                    Console.WriteLine("Firebase Admin SDK initialized successfully.");
                }
                
                _messaging = FirebaseMessaging.DefaultInstance;
                _isInitialized = true;
                Console.WriteLine("Firebase Messaging service initialized successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Firebase initialization error: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                // Continue without Firebase - we'll log warnings when methods are called
            }
        }

        public async Task<string> SendNotificationAsync(string token, string title, string body, Dictionary<string, string>? data = null)
        {
            if (!_isInitialized || _messaging == null)
            {
                Console.WriteLine("Firebase is not initialized. Notification not sent.");
                return "Firebase not initialized";
            }

            var message = new Message
            {
                Token = token,
                Notification = new Notification
                {
                    Title = title,
                    Body = body
                },
                Data = data
            };

            try
            {
                return await _messaging.SendAsync(message);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to send notification: {ex.Message}");
                return $"Error: {ex.Message}";
            }
        }

        public async Task<BatchResponse?> SendMulticastNotificationAsync(List<string> tokens, string title, string body, Dictionary<string, string>? data = null)
        {
            if (!_isInitialized || _messaging == null)
            {
                Console.WriteLine("Firebase is not initialized. Multicast notification not sent.");
                return null;
            }

            var message = new MulticastMessage
            {
                Tokens = tokens,
                Notification = new Notification
                {
                    Title = title,
                    Body = body
                },
                Data = data
            };

            try
            {
                return await _messaging.SendMulticastAsync(message);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to send multicast notification: {ex.Message}");
                return null;
            }
        }
    }
} 