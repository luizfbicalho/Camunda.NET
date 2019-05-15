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
namespace org.camunda.bpm.engine.cdi.test.impl.util
{
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.core.Is.@is;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.core.IsEqual.equalTo;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.core.IsInstanceOf.instanceOf;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.*;


	using ProcessEngineServicesProducer = org.camunda.bpm.engine.cdi.impl.ProcessEngineServicesProducer;
	using ProgrammaticBeanLookup = org.camunda.bpm.engine.cdi.impl.util.ProgrammaticBeanLookup;
	using SpecializedTestBean = org.camunda.bpm.engine.cdi.test.impl.beans.SpecializedTestBean;
	using Deployer = org.jboss.arquillian.container.test.api.Deployer;
	using Deployment = org.jboss.arquillian.container.test.api.Deployment;
	using Arquillian = org.jboss.arquillian.junit.Arquillian;
	using ArquillianResource = org.jboss.arquillian.test.api.ArquillianResource;
	using ShrinkWrap = org.jboss.shrinkwrap.api.ShrinkWrap;
	using JavaArchive = org.jboss.shrinkwrap.api.spec.JavaArchive;
	using Test = org.junit.Test;
	using RunWith = org.junit.runner.RunWith;

	/// 
	/// <summary>
	/// @author Ronny Bräunlich
	/// 
	/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @RunWith(Arquillian.class) public class ProgrammaticBeanLookupTest
	public class ProgrammaticBeanLookupTest
	{
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @ArquillianResource private org.jboss.arquillian.container.test.api.Deployer deployer;
		private Deployer deployer;

	  [Deployment(name : "normal", managed : false)]
	  public static JavaArchive createDeployment()
	  {
		return ShrinkWrap.create(typeof(JavaArchive)).addClass(typeof(ProgrammaticBeanLookup)).addClass(typeof(ProcessEngineServicesProducer)).addAsManifestResource("org/camunda/bpm/engine/cdi/test/impl/util/beans.xml", "beans.xml");
	  }

	  [Deployment(name : "withAlternative", managed : false)]
	  public static JavaArchive createDeploymentWithAlternative()
	  {
		return ShrinkWrap.create(typeof(JavaArchive)).addClass(typeof(ProgrammaticBeanLookup)).addClass(typeof(ProcessEngineServicesProducer)).addClass(typeof(AlternativeTestBean)).addAsManifestResource("org/camunda/bpm/engine/cdi/test/impl/util/beansWithAlternative.xml", "beans.xml");
	  }

	  [Deployment(name : "withSpecialization", managed : false)]
	  public static JavaArchive createDeploymentWithSpecialization()
	  {
		return ShrinkWrap.create(typeof(JavaArchive)).addClass(typeof(ProgrammaticBeanLookup)).addClass(typeof(ProcessEngineServicesProducer)).addClass(typeof(SpecializedTestBean)).addAsManifestResource("org/camunda/bpm/engine/cdi/test/impl/util/beans.xml", "beans.xml");
	  }

	  [Deployment(name : "withProducerMethod", managed : false)]
	  public static JavaArchive createDeploymentWithProducerMethod()
	  {
		return ShrinkWrap.create(typeof(JavaArchive)).addClass(typeof(ProgrammaticBeanLookup)).addClass(typeof(ProcessEngineServicesProducer)).addClass(typeof(BeanWithProducerMethods)).addAsManifestResource("org/camunda/bpm/engine/cdi/test/impl/util/beans.xml", "beans.xml");
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testLookupBean()
	  public virtual void testLookupBean()
	  {
		deployer.deploy("normal");
		object lookup = ProgrammaticBeanLookup.lookup("testOnly");
		assertThat(lookup, instanceOf(typeof(TestBean)));
		deployer.undeploy("normal");
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testLookupShouldFindAlternative()
	  public virtual void testLookupShouldFindAlternative()
	  {
		deployer.deploy("withAlternative");
		object lookup = ProgrammaticBeanLookup.lookup("testOnly");
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
		assertThat(lookup.GetType().FullName, @is(equalTo(typeof(AlternativeTestBean).FullName)));
		deployer.undeploy("withAlternative");
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testLookupShouldFindSpecialization()
	  public virtual void testLookupShouldFindSpecialization()
	  {
		deployer.deploy("withSpecialization");
		object lookup = ProgrammaticBeanLookup.lookup("testOnly");
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
		assertThat(lookup.GetType().FullName, @is(equalTo(typeof(SpecializedTestBean).FullName)));
		deployer.undeploy("withSpecialization");
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testLookupShouldSupportProducerMethods()
	  public virtual void testLookupShouldSupportProducerMethods()
	  {
		deployer.deploy("withProducerMethod");
		assertEquals("exampleString", ProgrammaticBeanLookup.lookup("producedString"));
		deployer.undeploy("withProducerMethod");
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Named("testOnly") public static class TestBean extends Object
	  public class TestBean : object
	  {
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Alternative @Named("testOnly") public static class AlternativeTestBean extends TestBean
	  public class AlternativeTestBean : TestBean
	  {
	  }
	}
}