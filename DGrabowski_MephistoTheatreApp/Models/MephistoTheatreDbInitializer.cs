﻿using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Web;

namespace DGrabowski_MephistoTheatreApp.Models
{
    public class MephistoTheatreDbInitializer : DropCreateDatabaseAlways<MephistoTheatreDbContext>
    {
        protected override void Seed(MephistoTheatreDbContext context)
        {
            SeedRoles(context);
            SeedUsers(context);
            SeedCategories(context);
            SeedStaffMembers(context);
            SeedMembers(context);
            SeedPosts(context);
            SeedComments(context);

            context.SaveChanges();
        }

        private void SeedRoles(MephistoTheatreDbContext context)
        {
            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));
            SeedRole(roleManager, "Admin");
            SeedRole(roleManager, "Staff");
            SeedRole(roleManager, "Member");
        }

        private void SeedRole(RoleManager<IdentityRole> roleManager, string roleName)
        {
            if (!roleManager.RoleExists(roleName))
            {
                roleManager.Create(new IdentityRole(roleName));
            }
        }

        private void SeedUsers(MephistoTheatreDbContext context)
        {
            var userManager = new UserManager<User>(new UserStore<User>(context));
            SeedAdminUser(userManager);
            SeedStaffUsers(userManager);
            SeedMemberUsers(userManager);
        }

        private void SeedAdminUser(UserManager<User> userManager)
        {
            var adminEmail = "admin@mephisto.com";
            if (userManager.FindByName(adminEmail) == null)
            {
                var adminUser = new Staff
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    FirstName = "Adam",
                    LastName = "Mada",
                    Street = "12 Mephisto way",
                    City = "Las Vegas",
                    PostCode = "LL66 6GG",
                    RegisteredAt = DateTime.Now.AddYears(-5),
                    EmploymentStartDate = DateTime.Now.AddYears(-5),
                    EmailConfirmed = true,
                    IsSuspended = false,
                };

                // Super liberal password validation for password for seeds
                userManager.PasswordValidator = new PasswordValidator
                {
                    RequireDigit = false,
                    RequiredLength = 1,
                    RequireLowercase = false,
                    RequireNonLetterOrDigit = false,
                    RequireUppercase = false,
                };

                // Hash and store the admin password
                var result = userManager.Create(adminUser, "admin123");

                if (result.Succeeded)
                {
                    // Assign the admin role to the admin user
                    userManager.AddToRole(adminUser.Id, "Admin");
                }
                else
                {
                    // Handle errors if user creation fails
                    // For example, log the errors or throw an exception
                    foreach (var error in result.Errors)
                    {
                        // Handle the error...
                    }
                }
            }
        }

        private void SeedStaffUsers(UserManager<User> userManager)
        {
            var staffUsers = new List<Staff>
            {
                new Staff
                {
                    UserName = "jeff@mephisto.com",
                    Email = "jeff@mephisto.com",
                    FirstName = "Jeff",
                    LastName = "Ffej",
                    Street = "14 Mephisto road",
                    City = "Las Vegas",
                    PostCode = "LL66 6GG",
                    RegisteredAt = DateTime.Now.AddYears(-2),
                    EmploymentStartDate = DateTime.Now.AddYears(-5),
                    EmailConfirmed = true,
                    IsSuspended = false,
                },
                new Staff
                {
                    UserName = "rick@mephisto.com",
                    Email = "rick@mephisto.com",
                    FirstName = "Rick",
                    LastName = "Kcir",
                    Street = "10 Mephisto ave",
                    City = "Las Vegas",
                    PostCode = "LL66 6GG",
                    RegisteredAt = DateTime.Now.AddYears(-2),
                    EmploymentStartDate = DateTime.Now.AddYears(-5),
                    EmailConfirmed = true,
                    IsSuspended = false,
                },
                // Add more staff users here as needed
            };

            foreach (var staffUser in staffUsers)
            {
                if (userManager.FindByName(staffUser.UserName) == null)
                {
                    staffUser.UserName = staffUser.UserName.ToLower(); // Ensure usernames are lowercase for consistency
                    userManager.Create(staffUser, "staff");
                    userManager.AddToRole(staffUser.Id, "Staff");
                }
            }
        }

        private void SeedMemberUsers(UserManager<User> userManager)
        {
            var memberUsers = new List<Member>
            {
                new Member
                {
                    UserName = "bill@gmail.com",
                    Email = "bill@gmail.com",
                    FirstName = "Bill",
                    LastName = "Llib",
                    Street = "10 members road",
                    City = "Memberslavia",
                    PostCode = "MEM BER",
                    RegisteredAt = DateTime.Now.AddMonths(-2),
                    EmailConfirmed = true,
                    IsSuspended = false,
                },
                new Member
                {
                    UserName = "bob@gmail.com",
                    Email = "bob@gmail.com",
                    FirstName = "Bob",
                    LastName = "Bob",
                    Street = "10 almostmembers road",
                    City = "Memberiaa",
                    PostCode = "MEM REB",
                    RegisteredAt = DateTime.Now.AddMonths(-5),
                    EmailConfirmed = true,
                    IsSuspended = false,
                },
                new Member
                {
                    UserName = "steve@gmail.com",
                    Email = "steve@gmail.com",
                    FirstName = "Steve",
                    LastName = "Evets",
                    Street = "10 members ave",
                    City = "Memberiada",
                    PostCode = "MEM REB",
                    RegisteredAt = DateTime.Now.AddMonths(-1),
                    EmailConfirmed = true,
                    IsSuspended = false,
                },
                new Member
                {
                    UserName = "suspended@gmail.com",
                    Email = "suspended@gmail.com",
                    FirstName = "Suspended",
                    LastName = "Suspended",
                    Street = "10 suspended road",
                    City = "Suspendedia",
                    PostCode = "SUS PEND",
                    RegisteredAt = DateTime.Now.AddMonths(-5),
                    EmailConfirmed = true,
                    IsSuspended = true,
                },
                // Add more member users here as needed
            };

            foreach (var memberUser in memberUsers)
            {
                if (userManager.FindByName(memberUser.UserName) == null)
                {
                    memberUser.UserName = memberUser.UserName.ToLower(); // Ensure usernames are lowercase for consistency
                    userManager.Create(memberUser, "member");
                    userManager.AddToRole(memberUser.Id, "Member");
                }
            }
        }

        private void SeedCategories(MephistoTheatreDbContext context)
        {
            var categories = new List<Category>
            {
                new Category { CategoryName = "Announcements" },
                new Category { CategoryName = "Movie Posts" },
                new Category { CategoryName = "Reviews" },
                // Add more categories here as needed
            };

            categories.ForEach(c => context.Categories.AddOrUpdate(p => p.CategoryName, c));
            context.SaveChanges();
        }

        private void SeedStaffMembers(MephistoTheatreDbContext context)
        {
            var staffMembers = new List<Staff>
            {
                new Staff
                {
                    UserName = "staff1",
                    FirstName = "John",
                    LastName = "Doe",
                    Street = "123 Main St",
                    City = "Cityville",
                    PostCode = "12345",
                    RegisteredAt = DateTime.Now,
                    EmploymentStartDate = DateTime.Now,
                },
                // Add more staff members here as needed
            };

            staffMembers.ForEach(s => context.Staffs.AddOrUpdate(u => u.UserName, s));
            context.SaveChanges();
        }

        private void SeedMembers(MephistoTheatreDbContext context)
            {
                var members = new List<Member>
                {
                    new Member
                    {
                        UserName = "member1",
                        FirstName = "Alice",
                        LastName = "Smith",
                        Street = "456 Elm St",
                        City = "Townsville",
                        PostCode = "54321",
                        RegisteredAt = DateTime.Now,
                    },
                    // Add more members as needed
                };

            members.ForEach(m => context.Members.AddOrUpdate(u => u.UserName, m));
            context.SaveChanges();
        }

        private void SeedPosts(MephistoTheatreDbContext context)
        {
                var posts = new List<Post>
                {
                    new Post
                    {
                        Title = "Example Post 1",
                        CreatedAt = DateTime.Now,
                        Body = "This is the body of the post.",
                        IsPublished = true,
                        IsArchived = false,
                        LastEditAt = null,
                        IsDraft = false,
                        StaffId = context.Staffs.First().Id, // Assuming you have staff members seeded
                        CategoryId = context.Categories.First(c => c.CategoryName == "Announcements").CategoryId, // Assuming you have categories seeded
                    },
                    // Add more posts as needed
                };

            posts.ForEach(p => context.Posts.AddOrUpdate(post => post.Title, p));
            context.SaveChanges();
        }

        private void SeedComments(MephistoTheatreDbContext context)
        {
            var comments = new List<Comment>
            {
                new Comment
                {
                    TimeStamp = DateTime.Now,
                    Body = "This is a first comment on the post.",
                    IsDraft = false,
                    IsPublished = true,
                    UserId = context.Members.First().Id, 
                    PostId = context.Posts.First().PostId, 
                },
                new Comment
                {
                    TimeStamp = DateTime.Now,
                    Body = "This is a second comment on the post.",
                    IsDraft = false,
                    IsPublished = true,
                    UserId = context.Members.First().Id, 
                    PostId = context.Posts.First().PostId,
                },
                new Comment
                {
                    TimeStamp = DateTime.Now,
                    Body = "This is a third comment on the post.",
                    IsDraft = false,
                    IsPublished = true,
                    UserId = context.Members.First().Id,
                    PostId = context.Posts.First().PostId,
                },
                // Add more comments as needed
            };

            comments.ForEach(c => context.Comments.AddOrUpdate(comment => comment.Body, c));
            context.SaveChanges();
        }
    }
}