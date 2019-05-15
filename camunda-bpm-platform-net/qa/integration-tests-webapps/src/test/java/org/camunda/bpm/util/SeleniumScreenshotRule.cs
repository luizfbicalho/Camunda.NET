using System;

/*
 * Copyright Camunda Services GmbH and/or licensed to Camunda Services GmbH
 * under one or more contributor license agreements. See the NOTICE file
 * distributed with this work for additional information regarding copyright
 * ownership. Camunda licenses this file to you under the Apache License,
 * Version 2.0; you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */
namespace org.camunda.bpm.util
{
	using FileUtils = org.apache.commons.io.FileUtils;
	using TestRule = org.junit.rules.TestRule;
	using Description = org.junit.runner.Description;
	using Statement = org.junit.runners.model.Statement;
	using OutputType = org.openqa.selenium.OutputType;
	using TakesScreenshot = org.openqa.selenium.TakesScreenshot;
	using WebDriver = org.openqa.selenium.WebDriver;
	using Augmenter = org.openqa.selenium.remote.Augmenter;
	using RemoteWebDriver = org.openqa.selenium.remote.RemoteWebDriver;


	/// <summary>
	/// Allows to take screenshots in case of an selenium test error.
	/// </summary>
	public class SeleniumScreenshotRule : TestRule
	{

	  private static Logger log = Logger.AnonymousLogger;

	  protected internal WebDriver webDriver;

	  public SeleniumScreenshotRule(WebDriver driver)
	  {
		if (driver is RemoteWebDriver)
		{
		  webDriver = (new Augmenter()).augment(driver);
		}
		else
		{
		  webDriver = driver;
		}
	  }

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: @Override public org.junit.runners.model.Statement apply(final org.junit.runners.model.Statement super, final org.junit.runner.Description description)
	  public override Statement apply(Statement @base, Description description)
	  {
		return new StatementAnonymousInnerClass(this, @base, description);
	  }

	  private class StatementAnonymousInnerClass : Statement
	  {
		  private readonly SeleniumScreenshotRule outerInstance;

		  private Statement @base;
		  private Description description;

		  public StatementAnonymousInnerClass(SeleniumScreenshotRule outerInstance, Statement @base, Description description)
		  {
			  this.outerInstance = outerInstance;
			  this.@base = @base;
			  this.description = description;
		  }


//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public void evaluate() throws Throwable
		  public override void evaluate()
		  {
			try
			{
			  @base.evaluate();
			}
			catch (Exception t)
			{
			  captureScreenShot(description);
			  throw t;
			}
		  }

		  public void captureScreenShot(Description describe)
		  {
			File scrFile = ((TakesScreenshot) outerInstance.webDriver).getScreenshotAs(OutputType.FILE);
			string now = (new SimpleDateFormat("yyyyMMdd-HHmmss")).format(DateTime.Now);
			string scrFilename = describe.ClassName + "-" + describe.MethodName + "-" + now + ".png";
			File outputFile = new File(computeScreenshotsRoot(describe.TestClass), scrFilename);
			log.info(scrFilename + " screenshot created.");
			try
			{
			  FileUtils.copyFile(scrFile, outputFile);
			}
			catch (IOException)
			{
			  log.severe("Error copying screenshot after exception.");
			}
		  }

		  public File computeScreenshotsRoot(Type anyTestClass)
		  {
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final String clsUri = anyTestClass.getName().replace('.','/') + ".class";
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
			string clsUri = anyTestClass.FullName.Replace('.','/') + ".class";
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.net.URL url = anyTestClass.getClassLoader().getResource(clsUri);
			URL url = anyTestClass.ClassLoader.getResource(clsUri);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final String clsPath = url.getPath();
			string clsPath = url.Path;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.io.File root = new java.io.File(clsPath.substring(0, clsPath.length() - clsUri.length()));
			File root = new File(clsPath.Substring(0, clsPath.Length - clsUri.Length));
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.io.File clsFile = new java.io.File(root, clsUri);
			File clsFile = new File(root, clsUri);
			return new File(root.ParentFile, "screenshots");
		  }

	  }
	}

}