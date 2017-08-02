﻿using System;
using NOpenPage.Configuration;
using OpenQA.Selenium;

namespace NOpenPage
{
    /// <summary>
    ///     An entry point for NOpenPage.
    ///     - Configures integration with Selenium WebDriver
    ///     - Serves as a factory for user defined page classes
    /// </summary>
    public class Browser
    {
        private Lazy<BrowserContext> _context;

        public Browser()
        {
            _context = new Lazy<BrowserContext>(() => new BrowserContextBuilder().Build());
        }

        /// <summary>
        ///     Configure Browser before start using NOpenPage
        /// </summary>
        /// <param name="action">A configuration to be applied</param>
        public void Configure(Action<IBrowserConfiguration> action)
        {
            Guard.NotNull(nameof(action), action);
            var builder = new BrowserContextBuilder();
            action(builder);
            _context = new Lazy<BrowserContext>(() => builder.Build());
        }

        /// <summary>
        ///     Create a page class of <typeparamref name="T" /> with a current <see cref="PageContext" />.
        /// </summary>
        /// <typeparam name="T">A type of a page class to create</typeparam>
        /// <returns>
        ///     A new instance of <typeparamref name="T" />
        /// </returns>
        public T On<T>() where T : Page
        {
            return CreatePage<T>();
        }

        public void Do(Action<IWebDriver> action)
        {
            _context.Value.Do(action);
        }

        private T CreatePage<T>() where T : Page
        {
            var context = _context.Value;
            var pageContext = context.CreatePageContext();
            return (T) Activator.CreateInstance(typeof(T), pageContext);
        }
    }
}