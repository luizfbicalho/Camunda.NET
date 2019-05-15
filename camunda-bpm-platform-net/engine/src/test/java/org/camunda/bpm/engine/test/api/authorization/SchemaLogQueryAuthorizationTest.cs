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
namespace org.camunda.bpm.engine.test.api.authorization
{
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.CoreMatchers.@is;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.Matchers.greaterThan;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertThat;

	using Groups = org.camunda.bpm.engine.authorization.Groups;
	using After = org.junit.After;
	using Test = org.junit.Test;

	/// <summary>
	/// @author Miklas Boskamp
	/// 
	/// </summary>
	public class SchemaLogQueryAuthorizationTest : AuthorizationTest
	{

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @After public void tearDown()
	  public override void tearDown()
	  {
		base.tearDown();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSimpleQueryWithoutAuthorization()
	  public virtual void testSimpleQueryWithoutAuthorization()
	  {
		// given

		// then
		assertThat(managementService.createSchemaLogQuery().list().size(), @is(0));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testCountQueryWithAuthorization()
	  public virtual void testCountQueryWithAuthorization()
	  {
		// given
		identityService.setAuthentication(userId, Collections.singletonList(org.camunda.bpm.engine.authorization.Groups_Fields.CAMUNDA_ADMIN));

		// then
		assertThat(managementService.createSchemaLogQuery().count(), @is(greaterThan(0L)));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryWithAuthorization()
	  public virtual void testQueryWithAuthorization()
	  {
		// given
		identityService.setAuthentication(userId, Collections.singletonList(org.camunda.bpm.engine.authorization.Groups_Fields.CAMUNDA_ADMIN));

		// then
		assertThat(managementService.createSchemaLogQuery().list().size(), @is(greaterThan(0)));
	  }
	}

}