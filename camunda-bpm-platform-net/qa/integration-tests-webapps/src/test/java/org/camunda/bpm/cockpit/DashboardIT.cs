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
namespace org.camunda.bpm.cockpit
{

	using Test = org.junit.Test;
	using By = org.openqa.selenium.By;
	using WebElement = org.openqa.selenium.WebElement;
	using ExpectedConditions = org.openqa.selenium.support.ui.ExpectedConditions;
	using WebDriverWait = org.openqa.selenium.support.ui.WebDriverWait;



	public class DashboardIT : AbstractWebappUiIntegrationTest
	{

	  public DashboardIT() : base("/camunda/app/cockpit")
	  {
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testLogin() throws java.net.URISyntaxException
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public virtual void testLogin()
	  {
		driver.get(appUrl + "/#/login");

		WebDriverWait wait = new WebDriverWait(driver, 10);

		WebElement user = wait.until(ExpectedConditions.presenceOfElementLocated(By.cssSelector("input[type=\"text\"]")));
		user.sendKeys("demo");

		WebElement password = wait.until(ExpectedConditions.presenceOfElementLocated(By.cssSelector("input[type=\"password\"]")));
		password.sendKeys("demo");

		WebElement submit = wait.until(ExpectedConditions.presenceOfElementLocated(By.cssSelector("button[type=\"submit\"]")));
		submit.submit();

		wait.until(ExpectedConditions.textToBePresentInElementLocated(By.cssSelector(".deployed .processes .value"), "1"));
		wait.until(ExpectedConditions.textToBePresentInElementLocated(By.cssSelector(".deployed .processes .stats-label"), "Process Definitions"));

		wait.until(currentURIIs(new URI(appUrl + "/default/#/dashboard")));
	  }

	}

}