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
namespace org.camunda.bpm.integrationtest.jboss
{

	using Deployment = org.jboss.arquillian.container.test.api.Deployment;
	using TargetsContainer = org.jboss.arquillian.container.test.api.TargetsContainer;
	using Arquillian = org.jboss.arquillian.junit.Arquillian;
	using ShrinkWrap = org.jboss.shrinkwrap.api.ShrinkWrap;
	using WebArchive = org.jboss.shrinkwrap.api.spec.WebArchive;
	using Assert = org.junit.Assert;
	using Test = org.junit.Test;
	using RunWith = org.junit.runner.RunWith;

	/// <summary>
	/// <para>Ensures subsystem boots in domain mode</para>
	/// 
	/// @author Daniel Meyer
	/// @author Christian Lipphardt
	/// 
	/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @RunWith(Arquillian.class) public class TestManagedDomain_JBOSS
	public class TestManagedDomain_JBOSS
	{
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment @TargetsContainer("test-domain") public static org.jboss.shrinkwrap.api.spec.WebArchive create1()
		public static WebArchive create1()
		{
		  return ShrinkWrap.create(typeof(WebArchive), "test.war");
		}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldBeAbleToLookupDefaultProcessEngine()
	  public virtual void shouldBeAbleToLookupDefaultProcessEngine()
	  {
		try
		{
		  Assert.assertNotNull(InitialContext.doLookup("java:global/camunda-bpm-platform/process-engine/default"));
		}
		catch (NamingException)
		{
		  Assert.fail("Could not lookup default process engine");
		}

		try
		{
		  Assert.assertNotNull(InitialContext.doLookup("java:global/camunda-bpm-platform/process-engine/someNonExistingEngine"));
		  Assert.fail("Should not be able to lookup someNonExistingEngine process engine");
		}
		catch (NamingException)
		{
		  // expected
		}
	  }

	}

}