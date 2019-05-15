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
namespace org.camunda.bpm
{
	using SeleniumScreenshotRule = org.camunda.bpm.util.SeleniumScreenshotRule;
	using TestUtil = org.camunda.bpm.util.TestUtil;
	using org.junit;
	using WebDriver = org.openqa.selenium.WebDriver;
	using ChromeDriver = org.openqa.selenium.chrome.ChromeDriver;
	using ChromeDriverService = org.openqa.selenium.chrome.ChromeDriverService;
	using ExpectedCondition = org.openqa.selenium.support.ui.ExpectedCondition;


	public class AbstractWebappUiIntegrationTest
	{

	  protected internal static WebDriver driver;

	  protected internal string appUrl;
	  protected internal TestProperties testProperties;
	  protected internal TestUtil testUtil;
	  protected internal string contextPath;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public org.camunda.bpm.util.SeleniumScreenshotRule screenshotRule = new org.camunda.bpm.util.SeleniumScreenshotRule(driver);
	  public SeleniumScreenshotRule screenshotRule = new SeleniumScreenshotRule(driver);

	  public AbstractWebappUiIntegrationTest(string contextPath)
	  {
		this.contextPath = contextPath;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @BeforeClass public static void createDriver()
	  public static void createDriver()
	  {
		string chromeDriverExecutable = "chromedriver";
		if (System.getProperty("os.name").ToLower(Locale.US).IndexOf("windows", StringComparison.Ordinal) > -1)
		{
		  chromeDriverExecutable += ".exe";
		}

		File chromeDriver = new File("target/chromedriver/" + chromeDriverExecutable);
		if (!chromeDriver.exists())
		{
		  throw new Exception("chromedriver could not be located!");
		}

		ChromeDriverService chromeDriverService = (new ChromeDriverService.Builder()).withVerbose(true).usingAnyFreePort().usingDriverExecutable(chromeDriver).build();

		driver = new ChromeDriver(chromeDriverService);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Before public void before() throws Exception
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public virtual void before()
	  {
		testProperties = new TestProperties(48080);
		appUrl = testProperties.getApplicationPath(contextPath);

		testUtil = new TestUtil(testProperties);
	  }

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: public static org.openqa.selenium.support.ui.ExpectedCondition<bool> currentURIIs(final java.net.URI pageURI)
	  public static ExpectedCondition<bool> currentURIIs(URI pageURI)
	  {

		return new ExpectedConditionAnonymousInnerClass(pageURI);

	  }

	  private class ExpectedConditionAnonymousInnerClass : ExpectedCondition<bool>
	  {
		  private URI pageURI;

		  public ExpectedConditionAnonymousInnerClass(URI pageURI)
		  {
			  this.pageURI = pageURI;
		  }

		  public override bool? apply(WebDriver webDriver)
		  {
			try
			{
			  return (new URI(webDriver.CurrentUrl)).Equals(pageURI);
			}
			catch (URISyntaxException)
			{
			  return false;
			}
		  }
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @After public void after()
	  public virtual void after()
	  {
		testUtil.destroy();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @AfterClass public static void quitDriver()
	  public static void quitDriver()
	  {
		driver.quit();
	  }

	}

}