﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ApplicationResources.UserInterface {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "16.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    public class EmailResources {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal EmailResources() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        public static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("ApplicationResources.UserInterface.EmailResources", typeof(EmailResources).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        public static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Error when attempting to send email..
        /// </summary>
        public static string EmailNotSend {
            get {
                return ResourceManager.GetString("EmailNotSend", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Sorry, but there was some issues and email was not sent..
        /// </summary>
        public static string EmailNotSent {
            get {
                return ResourceManager.GetString("EmailNotSent", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Email was sent successfully to user {0}..
        /// </summary>
        public static string EmailSend {
            get {
                return ResourceManager.GetString("EmailSend", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Hi {0}!&lt;/br&gt;&lt;/br&gt;
        ///
        ///This email was sent. because there was an request for resetting your password. Below you will find Password reset code - you need to paste it in password reset page in our website.
        ///
        ///This is your password reset key:&lt;/br&gt;
        ///
        ///&lt;strong&gt;{1}&lt;/strong&gt;
        ///
        ///The request was send at {2}, and is valid till {3}. Foremore, you have {4} attempts to reset your password. If all attempts are failed, this key will no longer be vallid and you will need to send a new reauest.:&lt;/br&gt;
        ///
        ///This message was gener [rest of string was truncated]&quot;;.
        /// </summary>
        public static string PasswordResetBody {
            get {
                return ResourceManager.GetString("PasswordResetBody", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Paswsord reset key.
        /// </summary>
        public static string PasswordResetSubject {
            get {
                return ResourceManager.GetString("PasswordResetSubject", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Hi {0}
        ///
        ///This is your email verification key:
        ///
        ///{1}
        ///
        ///Please use this code to verify your email address.
        ///
        ///This message was generated  and send automatically, so please do not respond to it.
        ///
        ///Greetings,
        ///Rubik&apos;s Cube Timer Team.
        /// </summary>
        public static string ResendVerificationCodeBody {
            get {
                return ResourceManager.GetString("ResendVerificationCodeBody", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Email address reverification.
        /// </summary>
        public static string ResendVerificationKeySybject {
            get {
                return ResourceManager.GetString("ResendVerificationKeySybject", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Hi {0}!
        ///
        ///We are glad that you decided to join our online Rubik&apos;s cube timer. We hope that you will spend here some great time and will improve your times in every category.&lt;br/&gt;
        ///
        ///This is your email verification key:
        ///
        ///{1}
        ///
        ///Please use this code to verify your email address when signing in for the first time only.
        ///
        ///This message was generated  and send automatically, so please do not respond to it.
        ///
        ///Greetings,
        ///Rubik&apos;s Cube Timer Team.
        /// </summary>
        public static string VerificationCodeBody {
            get {
                return ResourceManager.GetString("VerificationCodeBody", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Email address verification.
        /// </summary>
        public static string VerificationCodeSubject {
            get {
                return ResourceManager.GetString("VerificationCodeSubject", resourceCulture);
            }
        }
    }
}