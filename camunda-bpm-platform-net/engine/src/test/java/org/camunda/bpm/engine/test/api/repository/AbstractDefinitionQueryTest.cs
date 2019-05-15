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
namespace org.camunda.bpm.engine.test.api.repository
{
	using PluggableProcessEngineTestCase = org.camunda.bpm.engine.impl.test.PluggableProcessEngineTestCase;
	using Query = org.camunda.bpm.engine.query.Query;

	public abstract class AbstractDefinitionQueryTest : PluggableProcessEngineTestCase
	{

	  protected internal string deploymentOneId;
	  protected internal string deploymentTwoId;

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override protected void setUp() throws Exception
	  protected internal override void setUp()
	  {
		deploymentOneId = repositoryService.createDeployment().name("firstDeployment").addClasspathResource(ResourceOnePath).addClasspathResource(ResourceTwoPath).deploy().Id;

		deploymentTwoId = repositoryService.createDeployment().name("secondDeployment").addClasspathResource(ResourceOnePath).deploy().Id;

		base.setUp();
	  }

	  protected internal abstract string ResourceOnePath {get;}

	  protected internal abstract string ResourceTwoPath {get;}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override protected void tearDown() throws Exception
	  protected internal override void tearDown()
	  {
		base.tearDown();
		repositoryService.deleteDeployment(deploymentOneId, true);
		repositoryService.deleteDeployment(deploymentTwoId, true);
	  }

	  protected internal virtual void verifyQueryResults(Query query, int countExpected)
	  {
		assertEquals(countExpected, query.list().size());
		assertEquals(countExpected, query.count());

		if (countExpected == 1)
		{
		  assertNotNull(query.singleResult());
		}
		else if (countExpected > 1)
		{
		  verifySingleResultFails(query);
		}
		else if (countExpected == 0)
		{
		  assertNull(query.singleResult());
		}
	  }

	  private void verifySingleResultFails(Query query)
	  {
		try
		{
		  query.singleResult();
		  fail("Exception expected");
		}
		catch (ProcessEngineException)
		{
		  // expected exception
		}
	  }
	}

}