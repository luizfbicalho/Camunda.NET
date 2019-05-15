using System;
using System.Collections.Generic;

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
namespace org.camunda.bpm.container.impl.jmx.kernel
{


	using TestCase = junit.framework.TestCase;

	using FailingDeploymentOperationStep = org.camunda.bpm.container.impl.jmx.kernel.util.FailingDeploymentOperationStep;
	using StartServiceDeploymentOperationStep = org.camunda.bpm.container.impl.jmx.kernel.util.StartServiceDeploymentOperationStep;
	using StopServiceDeploymentOperationStep = org.camunda.bpm.container.impl.jmx.kernel.util.StopServiceDeploymentOperationStep;
	using TestService = org.camunda.bpm.container.impl.jmx.kernel.util.TestService;
	using TestServiceType = org.camunda.bpm.container.impl.jmx.kernel.util.TestServiceType;
	using PlatformService = org.camunda.bpm.container.impl.spi.PlatformService;

	/// <summary>
	/// Testcases for the <seealso cref="MBeanServiceContainer"/> Kernel.
	/// 
	/// @author Daniel Meyer
	/// 
	/// </summary>
	public class MBeanServiceContainerTest : TestCase
	{
		private bool InstanceFieldsInitialized = false;

		public MBeanServiceContainerTest()
		{
			if (!InstanceFieldsInitialized)
			{
				InitializeInstanceFields();
				InstanceFieldsInitialized = true;
			}
		}

		private void InitializeInstanceFields()
		{
			service1ObjectName = MBeanServiceContainer.getObjectName(service1Name);
			service2ObjectName = MBeanServiceContainer.getObjectName(service2Name);
			service3ObjectName = MBeanServiceContainer.getObjectName(service3Name);
			service4ObjectName = MBeanServiceContainer.getObjectName(service4Name);
		}


	  private MBeanServiceContainer serviceContainer;

	  private string service1Name = TestServiceType.TYPE1.TypeName + ":type=service1";
	  private string service2Name = TestServiceType.TYPE1.TypeName + ":type=service2";
	  private string service3Name = TestServiceType.TYPE2.TypeName + ":type=service3";
	  private string service4Name = TestServiceType.TYPE2.TypeName + ":type=service4";

	  private ObjectName service1ObjectName;
	  private ObjectName service2ObjectName;
	  private ObjectName service3ObjectName;
	  private ObjectName service4ObjectName;

	  private TestService service1 = new TestService();
	  private TestService service2 = new TestService();
	  private TestService service3 = new TestService();
	  private TestService service4 = new TestService();

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override protected void setUp() throws Exception
	  protected internal override void setUp()
	  {
		serviceContainer = new MBeanServiceContainer();
		base.setUp();
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected void tearDown() throws Exception
	  protected internal virtual void tearDown()
	  {
		// make sure all MBeans are removed after each test
		MBeanServer mBeanServer = serviceContainer.getmBeanServer();
		if (mBeanServer.isRegistered(service1ObjectName))
		{
		  mBeanServer.unregisterMBean(service1ObjectName);
		}
		if (mBeanServer.isRegistered(service2ObjectName))
		{
		  mBeanServer.unregisterMBean(service2ObjectName);
		}
		if (mBeanServer.isRegistered(service3ObjectName))
		{
		  mBeanServer.unregisterMBean(service3ObjectName);
		}
		if (mBeanServer.isRegistered(service4ObjectName))
		{
		  mBeanServer.unregisterMBean(service4ObjectName);
		}
		base.tearDown();
	  }

	  public virtual void testStartService()
	  {

		// initially the service is not present:
		assertNull(serviceContainer.getService(service1ObjectName));

		// we can start a service
		serviceContainer.startService(service1Name, service1);
		// and get it after that
		assertNotNull(serviceContainer.getService(service1ObjectName));
		assertEquals(service1, serviceContainer.getService(service1ObjectName));
		// as long it is started, I cannot start a second service with the same name:
		try
		{
		  serviceContainer.startService(service1Name, service1);
		  fail("exception expected");
		}
		catch (Exception e)
		{
		  assertTrue(e.Message.contains("service with same name already registered"));
		}

		// but, I can start a service with a different name:
		serviceContainer.startService(service2Name, service2);
		// and get it after that
		assertNotNull(serviceContainer.getService(service2ObjectName));

	  }

	  public virtual void testStopService()
	  {

		// start some service
		serviceContainer.startService(service1Name, service1);
		// it's there
		assertNotNull(serviceContainer.getService(service1ObjectName));

		// stop it:
		serviceContainer.stopService(service1Name);
		// now it's gone
		assertNull(serviceContainer.getService(service1ObjectName));

		try
		{
		  serviceContainer.stopService(service1Name);
		  fail("exception expected");
		}
		catch (Exception e)
		{
		  assertTrue(e.Message.contains("no such service registered"));
		}

	  }

	  public virtual void testGetServicesByType()
	  {

		serviceContainer.startService(service1Name, service1);
		serviceContainer.startService(service2Name, service2);

		IList<PlatformService<TestService>> servicesByType1 = serviceContainer.getServicesByType(TestServiceType.TYPE1);
		assertEquals(2, servicesByType1.Count);

		IList<PlatformService<TestService>> servicesByType2 = serviceContainer.getServicesByType(TestServiceType.TYPE2);
		assertEquals(0, servicesByType2.Count);

		serviceContainer.startService(service3Name, service3);
		serviceContainer.startService(service4Name, service4);

		servicesByType1 = serviceContainer.getServicesByType(TestServiceType.TYPE1);
		assertEquals(2, servicesByType1.Count);

		servicesByType2 = serviceContainer.getServicesByType(TestServiceType.TYPE2);
		assertEquals(2, servicesByType2.Count);

	  }

	  public virtual void testGetServiceValuesByType()
	  {

		// start some services
		serviceContainer.startService(service1Name, service1);
		serviceContainer.startService(service2Name, service2);

		IList<PlatformService<TestService>> servicesByType1 = serviceContainer.getServiceValuesByType(TestServiceType.TYPE1);
		assertEquals(2, servicesByType1.Count);
		assertTrue(servicesByType1.Contains(service1));
		assertTrue(servicesByType1.Contains(service2));

		IList<PlatformService<TestService>> servicesByType2 = serviceContainer.getServicesByType(TestServiceType.TYPE2);
		assertEquals(0, servicesByType2.Count);

		// start more services
		serviceContainer.startService(service3Name, service3);
		serviceContainer.startService(service4Name, service4);

		servicesByType1 = serviceContainer.getServicesByType(TestServiceType.TYPE1);
		assertEquals(2, servicesByType1.Count);

		servicesByType2 = serviceContainer.getServicesByType(TestServiceType.TYPE2);
		assertEquals(2, servicesByType2.Count);
		assertTrue(servicesByType2.Contains(service3));
		assertTrue(servicesByType2.Contains(service4));

	  }

	  public virtual void testGetServiceNames()
	  {

		// start some services
		serviceContainer.startService(service1Name, service1);
		serviceContainer.startService(service2Name, service2);

		ISet<string> serviceNames = serviceContainer.getServiceNames(TestServiceType.TYPE1);
		assertEquals(2, serviceNames.Count);
		assertTrue(serviceNames.Contains(service1Name));
		assertTrue(serviceNames.Contains(service2Name));

		serviceNames = serviceContainer.getServiceNames(TestServiceType.TYPE2);
		assertEquals(0, serviceNames.Count);

		// start more services
		serviceContainer.startService(service3Name, service3);
		serviceContainer.startService(service4Name, service4);

		serviceNames = serviceContainer.getServiceNames(TestServiceType.TYPE1);
		assertEquals(2, serviceNames.Count);
		assertTrue(serviceNames.Contains(service1Name));
		assertTrue(serviceNames.Contains(service2Name));

		serviceNames = serviceContainer.getServiceNames(TestServiceType.TYPE2);
		assertEquals(2, serviceNames.Count);
		assertTrue(serviceNames.Contains(service3Name));
		assertTrue(serviceNames.Contains(service4Name));

	  }

	  public virtual void testDeploymentOperation()
	  {

		serviceContainer.createDeploymentOperation("test op").addStep(new StartServiceDeploymentOperationStep(service1Name, service1)).addStep(new StartServiceDeploymentOperationStep(service2Name, service2)).execute();

		// both services were registered.
		assertEquals(service1, serviceContainer.getService(service1ObjectName));
		assertEquals(service2, serviceContainer.getService(service2ObjectName));

	  }

	  public virtual void testFailingDeploymentOperation()
	  {

		try
		{
		  serviceContainer.createDeploymentOperation("test failing op").addStep(new StartServiceDeploymentOperationStep(service1Name, service1)).addStep(new FailingDeploymentOperationStep()).addStep(new StartServiceDeploymentOperationStep(service2Name, service2)).execute();

		  fail("Exception expected");

		}
		catch (Exception e)
		{
		  assertTrue(e.Message.contains("Exception while performing 'test failing op' => 'failing step'"));

		}

		// none of the services were registered
		assertNull(serviceContainer.getService(service1ObjectName));
		assertNull(serviceContainer.getService(service2ObjectName));

		// different step ordering //////////////////////////////////

		try
		{
		  serviceContainer.createDeploymentOperation("test failing op").addStep(new StartServiceDeploymentOperationStep(service1Name, service1)).addStep(new StartServiceDeploymentOperationStep(service2Name, service2)).addStep(new FailingDeploymentOperationStep()).execute();

		  fail("Exception expected");

		}
		catch (Exception e)
		{
		  assertTrue(e.Message.contains("Exception while performing 'test failing op' => 'failing step'"));

		}

		// none of the services were registered
		assertNull(serviceContainer.getService(service1ObjectName));
		assertNull(serviceContainer.getService(service2ObjectName));

	  }

	  public virtual void testUndeploymentOperation()
	  {

		// lets first start some services:
		serviceContainer.startService(service1Name, service1);
		serviceContainer.startService(service2Name, service2);

		// run a composite undeployment operation
		serviceContainer.createUndeploymentOperation("test op").addStep(new StopServiceDeploymentOperationStep(service1Name)).addStep(new StopServiceDeploymentOperationStep(service2Name)).execute();

		// both services were stopped.
		assertNull(serviceContainer.getService(service1ObjectName));
		assertNull(serviceContainer.getService(service2ObjectName));

	  }

	  public virtual void testFailingUndeploymentOperation()
	  {

		// lets first start some services:
		serviceContainer.startService(service1Name, service1);
		serviceContainer.startService(service2Name, service2);

		// run a composite undeployment operation with a failing step
		serviceContainer.createUndeploymentOperation("test failing op").addStep(new StopServiceDeploymentOperationStep(service1Name)).addStep(new FailingDeploymentOperationStep()).addStep(new StopServiceDeploymentOperationStep(service2Name)).execute(); // this does not throw an exception even if some steps fail. (exceptions are logged)


		// both services were stopped.
		assertNull(serviceContainer.getService(service1ObjectName));
		assertNull(serviceContainer.getService(service2ObjectName));

		// different step ordering //////////////////////////////////

		serviceContainer.startService(service1Name, service1);
		serviceContainer.startService(service2Name, service2);

		// run a composite undeployment operation with a failing step
		serviceContainer.createUndeploymentOperation("test failing op").addStep(new FailingDeploymentOperationStep()).addStep(new StopServiceDeploymentOperationStep(service1Name)).addStep(new StopServiceDeploymentOperationStep(service2Name)).execute();

		// both services were stopped.
		assertNull(serviceContainer.getService(service1ObjectName));
		assertNull(serviceContainer.getService(service2ObjectName));

	  }

	}

}