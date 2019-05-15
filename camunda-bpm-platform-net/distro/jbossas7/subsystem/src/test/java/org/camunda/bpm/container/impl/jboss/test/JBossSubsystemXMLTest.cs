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
namespace org.camunda.bpm.container.impl.jboss.test
{
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertEquals;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertFalse;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertNotNull;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertTrue;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.fail;


	using ManagedProcessEngineMetadata = org.camunda.bpm.container.impl.jboss.config.ManagedProcessEngineMetadata;
	using Attribute = org.camunda.bpm.container.impl.jboss.extension.Attribute;
	using BpmPlatformExtension = org.camunda.bpm.container.impl.jboss.extension.BpmPlatformExtension;
	using Element = org.camunda.bpm.container.impl.jboss.extension.Element;
	using ModelConstants = org.camunda.bpm.container.impl.jboss.extension.ModelConstants;
	using MscManagedProcessEngineController = org.camunda.bpm.container.impl.jboss.service.MscManagedProcessEngineController;
	using ServiceNames = org.camunda.bpm.container.impl.jboss.service.ServiceNames;
	using ProcessEnginePluginXml = org.camunda.bpm.container.impl.metadata.spi.ProcessEnginePluginXml;
	using BpmPlatformPlugin = org.camunda.bpm.container.impl.plugin.BpmPlatformPlugin;
	using BpmPlatformPlugins = org.camunda.bpm.container.impl.plugin.BpmPlatformPlugins;
	using ProcessEngineException = org.camunda.bpm.engine.ProcessEngineException;
	using JobExecutor = org.camunda.bpm.engine.impl.jobexecutor.JobExecutor;
	using PathAddress = org.jboss.@as.controller.PathAddress;
	using PathElement = org.jboss.@as.controller.PathElement;
	using ModelDescriptionConstants = org.jboss.@as.controller.descriptions.ModelDescriptionConstants;
	using ContextNames = org.jboss.@as.naming.deployment.ContextNames;
	using AbstractSubsystemTest = org.jboss.@as.subsystem.test.AbstractSubsystemTest;
	using KernelServices = org.jboss.@as.subsystem.test.KernelServices;
	using ModelNode = org.jboss.dmr.ModelNode;
	using ModelType = org.jboss.dmr.ModelType;
	using ServiceContainer = org.jboss.msc.service.ServiceContainer;
	using ServiceController = org.jboss.msc.service.ServiceController;
	using ServiceName = org.jboss.msc.service.ServiceName;
	using Test = org.junit.Test;


	/// 
	/// <summary>
	/// @author nico.rehwaldt@camunda.com
	/// @author christian.lipphardt@camunda.com
	/// </summary>
	public class JBossSubsystemXMLTest : AbstractSubsystemTest
	{

	  public const string SUBSYSTEM_WITH_SINGLE_ENGINE = "subsystemWithSingleEngine.xml";
	  public const string SUBSYSTEM_WITH_ENGINES = "subsystemWithEngines.xml";
	  public const string SUBSYSTEM_WITH_ENGINES_PROPERTIES_PLUGINS_AND_JOB_EXECUTOR_WITH_EXPRESSIONS = "subsystemWithProcessEnginesPropertiesPluginsAndJobExecutorWithExpressions.xml";
	  public const string SUBSYSTEM_WITH_PROCESS_ENGINES_ELEMENT_ONLY = "subsystemWithProcessEnginesElementOnly.xml";
	  public const string SUBSYSTEM_WITH_ENGINES_AND_PROPERTIES = "subsystemWithEnginesAndProperties.xml";
	  public const string SUBSYSTEM_WITH_ENGINES_PROPERTIES_PLUGINS = "subsystemWithEnginesPropertiesPlugins.xml";
	  public const string SUBSYSTEM_WITH_DUPLICATE_ENGINE_NAMES = "subsystemWithDuplicateEngineNames.xml";
	  public const string SUBSYSTEM_WITH_JOB_EXECUTOR = "subsystemWithJobExecutor.xml";
	  public const string SUBSYSTEM_WITH_PROCESS_ENGINES_AND_JOB_EXECUTOR = "subsystemWithProcessEnginesAndJobExecutor.xml";
	  public const string SUBSYSTEM_WITH_JOB_EXECUTOR_AND_PROPERTIES = "subsystemWithJobExecutorAndProperties.xml";
	  public const string SUBSYSTEM_WITH_JOB_EXECUTOR_WITHOUT_ACQUISITION_STRATEGY = "subsystemWithJobExecutorAndWithoutAcquisitionStrategy.xml";

	  public const string LOCK_TIME_IN_MILLIS = "lockTimeInMillis";
	  public const string WAIT_TIME_IN_MILLIS = "waitTimeInMillis";
	  public const string MAX_JOBS_PER_ACQUISITION = "maxJobsPerAcquisition";

	  public static readonly ServiceName PLATFORM_SERVICE_NAME = ServiceNames.forMscRuntimeContainerDelegate();
	  public static readonly ServiceName PLATFORM_JOBEXECUTOR_SERVICE_NAME = ServiceNames.forMscExecutorService();

	  public static readonly ServiceName processEngineServiceBindingServiceName = ContextNames.GLOBAL_CONTEXT_SERVICE_NAME.append("camunda-bpm-platform").append("process-engine").append("ProcessEngineService!org.camunda.bpm.ProcessEngineService");

	  public JBossSubsystemXMLTest() : base(org.camunda.bpm.container.impl.jboss.extension.ModelConstants_Fields.SUBSYSTEM_NAME, new BpmPlatformExtension())
	  {
	  }

	  private static IDictionary<string, string> EXPRESSION_PROPERTIES = new Dictionary<string, string>();

	  static JBossSubsystemXMLTest()
	  {
		EXPRESSION_PROPERTIES["org.camunda.bpm.jboss.process-engine.test.isDefault"] = "true";
		EXPRESSION_PROPERTIES["org.camunda.bpm.jboss.process-engine.test.datasource"] = "java:jboss/datasources/ExampleDS";
		EXPRESSION_PROPERTIES["org.camunda.bpm.jboss.process-engine.test.history-level"] = "audit";
		EXPRESSION_PROPERTIES["org.camunda.bpm.jboss.process-engine.test.configuration"] = "org.camunda.bpm.container.impl.jboss.config.ManagedJtaProcessEngineConfiguration";
		EXPRESSION_PROPERTIES["org.camunda.bpm.jboss.process-engine.test.property.job-acquisition-name"] = "default";
		EXPRESSION_PROPERTIES["org.camunda.bpm.jboss.process-engine.test.plugin.ldap.class"] = "org.camunda.bpm.identity.impl.ldap.plugin.LdapIdentityProviderPlugin";
		EXPRESSION_PROPERTIES["org.camunda.bpm.jboss.process-engine.test.plugin.ldap.property.test"] = "abc";
		EXPRESSION_PROPERTIES["org.camunda.bpm.jboss.process-engine.test.plugin.ldap.property.number"] = "123";
		EXPRESSION_PROPERTIES["org.camunda.bpm.jboss.process-engine.test.plugin.ldap.property.bool"] = "true";
		EXPRESSION_PROPERTIES["org.camunda.bpm.jboss.job-executor.thread-pool-name"] = "job-executor-tp";
		EXPRESSION_PROPERTIES["org.camunda.bpm.jboss.job-executor.job-acquisition.default.acquisition-strategy"] = "SEQUENTIAL";
		EXPRESSION_PROPERTIES["org.camunda.bpm.jboss.job-executor.job-acquisition.default.property.lockTimeInMillis"] = "300000";
		EXPRESSION_PROPERTIES["org.camunda.bpm.jboss.job-executor.job-acquisition.default.property.waitTimeInMillis"] = "5000";
		EXPRESSION_PROPERTIES["org.camunda.bpm.jboss.job-executor.job-acquisition.default.property.maxJobsPerAcquisition"] = "3";
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testParseSubsystemXml() throws Exception
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public virtual void testParseSubsystemXml()
	  {
		string subsystemXml = FileUtils.readFile(SUBSYSTEM_WITH_PROCESS_ENGINES_ELEMENT_ONLY);

		IList<ModelNode> operations = parse(subsystemXml);

		assertEquals(1, operations.Count);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testParseSubsystemXmlWithEngines() throws Exception
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public virtual void testParseSubsystemXmlWithEngines()
	  {
		string subsystemXml = FileUtils.readFile(SUBSYSTEM_WITH_ENGINES);

		IList<ModelNode> operations = parse(subsystemXml);

		assertEquals(3, operations.Count);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testParseSubsystemXmlWithEnginesAndProperties() throws Exception
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public virtual void testParseSubsystemXmlWithEnginesAndProperties()
	  {
		string subsystemXml = FileUtils.readFile(SUBSYSTEM_WITH_ENGINES_AND_PROPERTIES);

		IList<ModelNode> operations = parse(subsystemXml);

		assertEquals(5, operations.Count);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testParseSubsystemXmlWithEnginesPropertiesPlugins() throws Exception
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public virtual void testParseSubsystemXmlWithEnginesPropertiesPlugins()
	  {
		string subsystemXml = FileUtils.readFile(SUBSYSTEM_WITH_ENGINES_PROPERTIES_PLUGINS);

		IList<ModelNode> operations = parse(subsystemXml);

		assertEquals(3, operations.Count);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testInstallSubsystemWithEnginesPropertiesPlugins() throws Exception
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public virtual void testInstallSubsystemWithEnginesPropertiesPlugins()
	  {
		string subsystemXml = FileUtils.readFile(SUBSYSTEM_WITH_ENGINES_PROPERTIES_PLUGINS);

		KernelServices services = createKernelServicesBuilder(null).setSubsystemXml(subsystemXml).build();

		ServiceContainer container = services.Container;

		assertNotNull("platform service should be installed", container.getRequiredService(PLATFORM_SERVICE_NAME));
		assertNotNull("process engine service should be bound in JNDI", container.getRequiredService(processEngineServiceBindingServiceName));

//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: org.jboss.msc.service.ServiceController<?> defaultEngineService = container.getService(org.camunda.bpm.container.impl.jboss.service.ServiceNames.forManagedProcessEngine("__default"));
		ServiceController<object> defaultEngineService = container.getService(ServiceNames.forManagedProcessEngine("__default"));

		assertNotNull("process engine controller for engine __default is installed ", defaultEngineService);

		ManagedProcessEngineMetadata metadata = ((MscManagedProcessEngineController) defaultEngineService.Service).ProcessEngineMetadata;
		IDictionary<string, string> configurationProperties = metadata.ConfigurationProperties;
		assertEquals("default", configurationProperties["job-name"]);
		assertEquals("default", configurationProperties["job-acquisition"]);
		assertEquals("default", configurationProperties["job-acquisition-name"]);

		IDictionary<string, string> foxLegacyProperties = metadata.FoxLegacyProperties;
		assertTrue(foxLegacyProperties.Count == 0);

		assertNotNull("process engine controller for engine __default is installed ", container.getRequiredService(ServiceNames.forManagedProcessEngine("__default")));
		assertNotNull("process engine controller for engine __test is installed ", container.getRequiredService(ServiceNames.forManagedProcessEngine("__test")));

		// check we have parsed the plugin configurations
		metadata = ((MscManagedProcessEngineController) container.getRequiredService(ServiceNames.forManagedProcessEngine("__test")).Service).ProcessEngineMetadata;
		IList<ProcessEnginePluginXml> pluginConfigurations = metadata.PluginConfigurations;

		ProcessEnginePluginXml processEnginePluginXml = pluginConfigurations[0];
		assertEquals("org.camunda.bpm.identity.impl.ldap.plugin.LdapIdentityProviderPlugin", processEnginePluginXml.PluginClass);
		IDictionary<string, string> processEnginePluginXmlProperties = processEnginePluginXml.Properties;
		assertEquals("abc", processEnginePluginXmlProperties["test"]);
		assertEquals("123", processEnginePluginXmlProperties["number"]);
		assertEquals("true", processEnginePluginXmlProperties["bool"]);

		processEnginePluginXml = pluginConfigurations[1];
		assertEquals("org.camunda.bpm.identity.impl.ldap.plugin.LdapIdentityProviderPlugin", processEnginePluginXml.PluginClass);
		processEnginePluginXmlProperties = processEnginePluginXml.Properties;
		assertEquals("cba", processEnginePluginXmlProperties["test"]);
		assertEquals("321", processEnginePluginXmlProperties["number"]);
		assertEquals("false", processEnginePluginXmlProperties["bool"]);

		// test correct subsystem removal
		assertRemoveSubsystemResources(services);
		try
		{
//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: org.jboss.msc.service.ServiceController<?> service = container.getRequiredService(org.camunda.bpm.container.impl.jboss.service.ServiceNames.forManagedProcessEngine("__default"));
		  ServiceController<object> service = container.getRequiredService(ServiceNames.forManagedProcessEngine("__default"));
		  fail("Service '" + service.Name + "' should have been removed.");
		}
		catch (Exception)
		{
		  // nop
		}
		try
		{
//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: org.jboss.msc.service.ServiceController<?> service = container.getRequiredService(org.camunda.bpm.container.impl.jboss.service.ServiceNames.forManagedProcessEngine("__test"));
		  ServiceController<object> service = container.getRequiredService(ServiceNames.forManagedProcessEngine("__test"));
		  fail("Service '" + service.Name + "' should have been removed.");
		}
		catch (Exception)
		{
		  // nop
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testInstallSubsystemXml() throws Exception
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public virtual void testInstallSubsystemXml()
	  {
		string subsystemXml = FileUtils.readFile(SUBSYSTEM_WITH_PROCESS_ENGINES_ELEMENT_ONLY);

		KernelServices services = createKernelServicesBuilder(null).setSubsystemXml(subsystemXml).build();

		ServiceContainer container = services.Container;
		assertNotNull("platform service should be installed", container.getService(PLATFORM_SERVICE_NAME));
		assertNotNull("process engine service should be bound in JNDI", container.getService(processEngineServiceBindingServiceName));

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testInstallSubsystemXmlPlatformPlugins() throws Exception
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public virtual void testInstallSubsystemXmlPlatformPlugins()
	  {
		string subsystemXml = FileUtils.readFile(SUBSYSTEM_WITH_PROCESS_ENGINES_ELEMENT_ONLY);

		KernelServices services = createKernelServicesBuilder(null).setSubsystemXml(subsystemXml).build();

		ServiceContainer container = services.Container;
//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: org.jboss.msc.service.ServiceController<?> serviceController = container.getService(org.camunda.bpm.container.impl.jboss.service.ServiceNames.forBpmPlatformPlugins());
		ServiceController<object> serviceController = container.getService(ServiceNames.forBpmPlatformPlugins());
		assertNotNull(serviceController);
		object platformPlugins = serviceController.Value;
		assertTrue(platformPlugins is BpmPlatformPlugins);
		assertNotNull(platformPlugins);
		IList<BpmPlatformPlugin> plugins = ((BpmPlatformPlugins) platformPlugins).Plugins;
		assertEquals(1, plugins.Count);
		assertTrue(plugins[0] is ExampleBpmPlatformPlugin);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testInstallSubsystemWithEnginesXml() throws Exception
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public virtual void testInstallSubsystemWithEnginesXml()
	  {
		string subsystemXml = FileUtils.readFile(SUBSYSTEM_WITH_ENGINES);

		KernelServices services = createKernelServicesBuilder(null).setSubsystemXml(subsystemXml).build();


		ServiceContainer container = services.Container;
		assertNotNull("platform service should be installed", container.getService(PLATFORM_SERVICE_NAME));
		assertNotNull("process engine service should be bound in JNDI", container.getService(processEngineServiceBindingServiceName));

		assertNotNull("process engine controller for engine __default is installed ", container.getService(ServiceNames.forManagedProcessEngine("__default")));
		assertNotNull("process engine controller for engine __test is installed ", container.getService(ServiceNames.forManagedProcessEngine("__test")));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testInstallSubsystemWithEnginesAndPropertiesXml() throws Exception
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public virtual void testInstallSubsystemWithEnginesAndPropertiesXml()
	  {
		string subsystemXml = FileUtils.readFile(SUBSYSTEM_WITH_ENGINES_AND_PROPERTIES);

		KernelServices services = createKernelServicesBuilder(null).setSubsystemXml(subsystemXml).build();
		ServiceContainer container = services.Container;


		assertNotNull("platform service should be installed", container.getService(PLATFORM_SERVICE_NAME));
		assertNotNull("process engine service should be bound in JNDI", container.getService(processEngineServiceBindingServiceName));

//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: org.jboss.msc.service.ServiceController<?> defaultEngineService = container.getService(org.camunda.bpm.container.impl.jboss.service.ServiceNames.forManagedProcessEngine("__default"));
		ServiceController<object> defaultEngineService = container.getService(ServiceNames.forManagedProcessEngine("__default"));

		assertNotNull("process engine controller for engine __default is installed ", defaultEngineService);

		ManagedProcessEngineMetadata metadata = ((MscManagedProcessEngineController) defaultEngineService.Service).ProcessEngineMetadata;
		IDictionary<string, string> configurationProperties = metadata.ConfigurationProperties;
		assertEquals("default", configurationProperties["job-name"]);
		assertEquals("default", configurationProperties["job-acquisition"]);
		assertEquals("default", configurationProperties["job-acquisition-name"]);

		IDictionary<string, string> foxLegacyProperties = metadata.FoxLegacyProperties;
		assertTrue(foxLegacyProperties.Count == 0);

		assertNotNull("process engine controller for engine __test is installed ", container.getService(ServiceNames.forManagedProcessEngine("__test")));
		assertNotNull("process engine controller for engine __emptyPropertiesTag is installed ", container.getService(ServiceNames.forManagedProcessEngine("__emptyPropertiesTag")));
		assertNotNull("process engine controller for engine __noPropertiesTag is installed ", container.getService(ServiceNames.forManagedProcessEngine("__noPropertiesTag")));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testInstallSubsystemWithDuplicateEngineNamesXml() throws Exception
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public virtual void testInstallSubsystemWithDuplicateEngineNamesXml()
	  {
		string subsystemXml = FileUtils.readFile(SUBSYSTEM_WITH_DUPLICATE_ENGINE_NAMES);

		try
		{
		  createKernelServicesBuilder(null).setSubsystemXml(subsystemXml).build();

		}
		catch (ProcessEngineException fpe)
		{
		  assertTrue("Duplicate process engine detected!", fpe.Message.contains("A process engine with name '__test' already exists."));
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testInstallSubsystemWithSingleEngineXml() throws Exception
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public virtual void testInstallSubsystemWithSingleEngineXml()
	  {
		string subsystemXml = FileUtils.readFile(SUBSYSTEM_WITH_SINGLE_ENGINE);

		KernelServices services = createKernelServicesBuilder(null).setSubsystemXml(subsystemXml).build();
		ServiceContainer container = services.Container;

		assertNotNull("platform service should be installed", container.getService(PLATFORM_SERVICE_NAME));
		assertNotNull("process engine service should be bound in JNDI", container.getService(processEngineServiceBindingServiceName));

		assertNotNull("process engine controller for engine __default is installed ", container.getService(ServiceNames.forManagedProcessEngine("__default")));

		string persistedSubsystemXml = services.PersistedSubsystemXml;
		compareXml(null, subsystemXml, persistedSubsystemXml);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testParseSubsystemWithJobExecutorXml() throws Exception
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public virtual void testParseSubsystemWithJobExecutorXml()
	  {
		string subsystemXml = FileUtils.readFile(SUBSYSTEM_WITH_JOB_EXECUTOR);
	//    System.out.println(normalizeXML(subsystemXml));

		IList<ModelNode> operations = parse(subsystemXml);
	//    System.out.println(operations);
		assertEquals(4, operations.Count);

		ModelNode jobExecutor = operations[1];
		PathAddress pathAddress = PathAddress.pathAddress(jobExecutor.get(ModelDescriptionConstants.OP_ADDR));
		assertEquals(2, pathAddress.size());

		PathElement element = pathAddress.getElement(0);
		assertEquals(ModelDescriptionConstants.SUBSYSTEM, element.Key);
		assertEquals(org.camunda.bpm.container.impl.jboss.extension.ModelConstants_Fields.SUBSYSTEM_NAME, element.Value);
		element = pathAddress.getElement(1);
		assertEquals(Element.JOB_EXECUTOR.LocalName, element.Key);
		assertEquals(Attribute.DEFAULT.LocalName, element.Value);

		assertEquals("job-executor-tp", jobExecutor.get(Element.THREAD_POOL_NAME.LocalName).asString());

		ModelNode jobAcquisition = operations[2];
		assertEquals("default", jobAcquisition.get(Attribute.NAME.LocalName).asString());
		assertEquals("SEQUENTIAL", jobAcquisition.get(Element.ACQUISITION_STRATEGY.LocalName).asString());

		jobAcquisition = operations[3];
		assertEquals("anders", jobAcquisition.get(Attribute.NAME.LocalName).asString());
		assertEquals("SEQUENTIAL", jobAcquisition.get(Element.ACQUISITION_STRATEGY.LocalName).asString());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testInstallSubsystemWithJobExecutorXml() throws Exception
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public virtual void testInstallSubsystemWithJobExecutorXml()
	  {
		string subsystemXml = FileUtils.readFile(SUBSYSTEM_WITH_JOB_EXECUTOR);
	//    System.out.println(normalizeXML(subsystemXml));
		KernelServices services = createKernelServicesBuilder(null).setSubsystemXml(subsystemXml).build();
		ServiceContainer container = services.Container;
	//    container.dumpServices();

		assertNotNull("platform service should be installed", container.getService(PLATFORM_SERVICE_NAME));
		assertNotNull("process engine service should be bound in JNDI", container.getService(processEngineServiceBindingServiceName));

		assertNotNull("platform jobexecutor service should be installed", container.getService(PLATFORM_JOBEXECUTOR_SERVICE_NAME));

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testParseSubsystemWithJobExecutorAndPropertiesXml() throws Exception
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public virtual void testParseSubsystemWithJobExecutorAndPropertiesXml()
	  {
		string subsystemXml = FileUtils.readFile(SUBSYSTEM_WITH_JOB_EXECUTOR_AND_PROPERTIES);

		IList<ModelNode> operations = parse(subsystemXml);
		assertEquals(5, operations.Count);

		// "default" job acquisition ///////////////////////////////////////////////////////////
		ModelNode jobAcquisition = operations[2];
		assertEquals("default", jobAcquisition.get(Attribute.NAME.LocalName).asString());

		// "anders" job acquisition ////////////////////////////////////////////////////////////
		jobAcquisition = operations[3];
		assertEquals("anders", jobAcquisition.get(Attribute.NAME.LocalName).asString());
		assertTrue(jobAcquisition.has(Element.PROPERTIES.LocalName));
		assertTrue(jobAcquisition.hasDefined(Element.PROPERTIES.LocalName));

		ModelNode properties = jobAcquisition.get(Element.PROPERTIES.LocalName);
		assertEquals(3, properties.asPropertyList().size());

		assertTrue(properties.has(LOCK_TIME_IN_MILLIS));
		assertTrue(properties.hasDefined(LOCK_TIME_IN_MILLIS));
		assertEquals(600000, properties.get(LOCK_TIME_IN_MILLIS).asInt());

		assertTrue(properties.has(WAIT_TIME_IN_MILLIS));
		assertTrue(properties.hasDefined(WAIT_TIME_IN_MILLIS));
		assertEquals(10000, properties.get(WAIT_TIME_IN_MILLIS).asInt());

		assertTrue(properties.has(MAX_JOBS_PER_ACQUISITION));
		assertTrue(properties.hasDefined(MAX_JOBS_PER_ACQUISITION));
		assertEquals(5, properties.get(MAX_JOBS_PER_ACQUISITION).asInt());

		// "mixed" job acquisition ////////////////////////////////////////////////////////////
		jobAcquisition = operations[4];
		assertEquals("mixed", jobAcquisition.get(Attribute.NAME.LocalName).asString());
		assertTrue(jobAcquisition.has(Element.PROPERTIES.LocalName));
		assertTrue(jobAcquisition.hasDefined(Element.PROPERTIES.LocalName));

		properties = jobAcquisition.get(Element.PROPERTIES.LocalName);
		assertEquals(1, properties.asPropertyList().size());

		assertTrue(properties.has(LOCK_TIME_IN_MILLIS));
		assertTrue(properties.hasDefined(LOCK_TIME_IN_MILLIS));
		assertEquals(500000, properties.get(LOCK_TIME_IN_MILLIS).asInt());

		assertFalse(properties.has(WAIT_TIME_IN_MILLIS));
		assertFalse(properties.hasDefined(WAIT_TIME_IN_MILLIS));
		assertFalse(properties.has(MAX_JOBS_PER_ACQUISITION));
		assertFalse(properties.hasDefined(MAX_JOBS_PER_ACQUISITION));

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testInstallSubsystemWithJobExecutorAndPropertiesXml() throws Exception
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public virtual void testInstallSubsystemWithJobExecutorAndPropertiesXml()
	  {
		string subsystemXml = FileUtils.readFile(SUBSYSTEM_WITH_JOB_EXECUTOR_AND_PROPERTIES);

		KernelServices services = createKernelServicesBuilder(null).setSubsystemXml(subsystemXml).build();
		ServiceContainer container = services.Container;

		assertNotNull("platform service should be installed", container.getService(PLATFORM_SERVICE_NAME));
		assertNotNull("process engine service should be bound in JNDI", container.getService(processEngineServiceBindingServiceName));

		assertNotNull("platform jobexecutor service should be installed", container.getService(PLATFORM_JOBEXECUTOR_SERVICE_NAME));

		// "default" job acquisition ///////////////////////////////////////////////////////////
//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: org.jboss.msc.service.ServiceController<?> defaultJobAcquisitionService = container.getService(org.camunda.bpm.container.impl.jboss.service.ServiceNames.forMscRuntimeContainerJobExecutorService("default"));
		ServiceController<object> defaultJobAcquisitionService = container.getService(ServiceNames.forMscRuntimeContainerJobExecutorService("default"));
		assertNotNull("platform job acquisition service 'default' should be installed", defaultJobAcquisitionService);

		object value = defaultJobAcquisitionService.Value;
		assertNotNull(value);
		assertTrue(value is JobExecutor);

		JobExecutor defaultJobExecutor = (JobExecutor) value;
		assertEquals(300000, defaultJobExecutor.LockTimeInMillis);
		assertEquals(5000, defaultJobExecutor.WaitTimeInMillis);
		assertEquals(3, defaultJobExecutor.MaxJobsPerAcquisition);

		// "anders" job acquisition /////////////////////////////////////////////////////////
//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: org.jboss.msc.service.ServiceController<?> andersJobAcquisitionService = container.getService(org.camunda.bpm.container.impl.jboss.service.ServiceNames.forMscRuntimeContainerJobExecutorService("anders"));
		ServiceController<object> andersJobAcquisitionService = container.getService(ServiceNames.forMscRuntimeContainerJobExecutorService("anders"));
		assertNotNull("platform job acquisition service 'anders' should be installed", andersJobAcquisitionService);

		value = andersJobAcquisitionService.Value;
		assertNotNull(value);
		assertTrue(value is JobExecutor);

		JobExecutor andersJobExecutor = (JobExecutor) value;
		assertEquals(600000, andersJobExecutor.LockTimeInMillis);
		assertEquals(10000, andersJobExecutor.WaitTimeInMillis);
		assertEquals(5, andersJobExecutor.MaxJobsPerAcquisition);

		// "mixed" job acquisition /////////////////////////////////////////////////////////
//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: org.jboss.msc.service.ServiceController<?> mixedJobAcquisitionService = container.getService(org.camunda.bpm.container.impl.jboss.service.ServiceNames.forMscRuntimeContainerJobExecutorService("mixed"));
		ServiceController<object> mixedJobAcquisitionService = container.getService(ServiceNames.forMscRuntimeContainerJobExecutorService("mixed"));
		assertNotNull("platform job acquisition service 'mixed' should be installed", mixedJobAcquisitionService);

		value = mixedJobAcquisitionService.Value;
		assertNotNull(value);
		assertTrue(value is JobExecutor);

		JobExecutor mixedJobExecutor = (JobExecutor) value;
		assertEquals(500000, mixedJobExecutor.LockTimeInMillis);
		// default values
		assertEquals(5000, mixedJobExecutor.WaitTimeInMillis);
		assertEquals(3, mixedJobExecutor.MaxJobsPerAcquisition);

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testJobAcquisitionStrategyOptional() throws Exception
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public virtual void testJobAcquisitionStrategyOptional()
	  {
		string subsystemXml = FileUtils.readFile(SUBSYSTEM_WITH_JOB_EXECUTOR_WITHOUT_ACQUISITION_STRATEGY);
	//    System.out.println(normalizeXML(subsystemXml));
		KernelServices services = createKernelServicesBuilder(null).setSubsystemXml(subsystemXml).build();
		ServiceContainer container = services.Container;
	//    container.dumpServices();

		assertNotNull("platform service should be installed", container.getService(PLATFORM_SERVICE_NAME));
		assertNotNull("process engine service should be bound in JNDI", container.getService(processEngineServiceBindingServiceName));

		assertNotNull("platform jobexecutor service should be installed", container.getService(PLATFORM_JOBEXECUTOR_SERVICE_NAME));
	  }


//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testParseSubsystemXmlWithEnginesAndJobExecutor() throws Exception
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public virtual void testParseSubsystemXmlWithEnginesAndJobExecutor()
	  {
		string subsystemXml = FileUtils.readFile(SUBSYSTEM_WITH_PROCESS_ENGINES_AND_JOB_EXECUTOR);
	//    System.out.println(normalizeXML(subsystemXml));

		IList<ModelNode> operations = parse(subsystemXml);
		assertEquals(6, operations.Count);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testInstallSubsystemXmlWithEnginesAndJobExecutor() throws Exception
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public virtual void testInstallSubsystemXmlWithEnginesAndJobExecutor()
	  {
		string subsystemXml = FileUtils.readFile(SUBSYSTEM_WITH_PROCESS_ENGINES_AND_JOB_EXECUTOR);
	//    System.out.println(normalizeXML(subsystemXml));
		KernelServices services = createKernelServicesBuilder(null).setSubsystemXml(subsystemXml).build();
		ServiceContainer container = services.Container;
	//    container.dumpServices();

		assertNotNull("platform service should be installed", container.getService(PLATFORM_SERVICE_NAME));
		assertNotNull("platform jobexecutor service should be installed", container.getService(PLATFORM_JOBEXECUTOR_SERVICE_NAME));
		assertNotNull("process engine service should be bound in JNDI", container.getService(processEngineServiceBindingServiceName));

		assertNotNull("process engine controller for engine __default is installed ", container.getService(ServiceNames.forManagedProcessEngine("__default")));
		assertNotNull("process engine controller for engine __test is installed ", container.getService(ServiceNames.forManagedProcessEngine("__test")));


		string persistedSubsystemXml = services.PersistedSubsystemXml;
	//    System.out.println(persistedSubsystemXml);
		compareXml(null, subsystemXml, persistedSubsystemXml);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testParseSubsystemXmlWithEnginePropertiesPluginsAndJobExecutorWithExpressions() throws Exception
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public virtual void testParseSubsystemXmlWithEnginePropertiesPluginsAndJobExecutorWithExpressions()
	  {
		string subsystemXml = FileUtils.readFile(SUBSYSTEM_WITH_ENGINES_PROPERTIES_PLUGINS_AND_JOB_EXECUTOR_WITH_EXPRESSIONS);

		IList<ModelNode> operations = parse(subsystemXml);

		assertEquals(4, operations.Count);
		// all elements with expression allowed should be an expression now
		assertExpressionType(operations[1], "default", "datasource", "history-level", "configuration");
		assertExpressionType(operations[1].get("properties"), "job-acquisition-name");
		assertExpressionType(operations[1].get("plugins").get(0), "class");
		assertExpressionType(operations[1].get("plugins").get(0).get("properties"), "test", "number", "bool");
		assertExpressionType(operations[2], "thread-pool-name");
		assertExpressionType(operations[3], "acquisition-strategy");
		assertExpressionType(operations[3].get("properties"), "lockTimeInMillis", "waitTimeInMillis", "maxJobsPerAcquisition");
		// all other elements should be string still
		assertStringType(operations[1], "name"); // process-engine name
		assertStringType(operations[3], "name"); // job-acquisition name
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testInstallSubsystemXmlWithEnginePropertiesPluginsAndJobExecutorWithExpressions() throws Exception
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public virtual void testInstallSubsystemXmlWithEnginePropertiesPluginsAndJobExecutorWithExpressions()
	  {
		System.Properties.putAll(EXPRESSION_PROPERTIES);
		try
		{
		  string subsystemXml = FileUtils.readFile(SUBSYSTEM_WITH_ENGINES_PROPERTIES_PLUGINS_AND_JOB_EXECUTOR_WITH_EXPRESSIONS);
		  KernelServices services = createKernelServicesBuilder(null).setSubsystemXml(subsystemXml).build();
		  ServiceContainer container = services.Container;

		  assertNotNull("platform service should be installed", container.getRequiredService(PLATFORM_SERVICE_NAME));
		  assertNotNull("process engine service should be bound in JNDI", container.getRequiredService(processEngineServiceBindingServiceName));

//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: org.jboss.msc.service.ServiceController<?> defaultEngineService = container.getService(org.camunda.bpm.container.impl.jboss.service.ServiceNames.forManagedProcessEngine("__test"));
		  ServiceController<object> defaultEngineService = container.getService(ServiceNames.forManagedProcessEngine("__test"));

		  assertNotNull("process engine controller for engine __test is installed ", defaultEngineService);

		  ManagedProcessEngineMetadata metadata = ((MscManagedProcessEngineController) defaultEngineService.Service).ProcessEngineMetadata;
		  IDictionary<string, string> configurationProperties = metadata.ConfigurationProperties;
		  assertEquals("default", configurationProperties["job-acquisition-name"]);

		  IDictionary<string, string> foxLegacyProperties = metadata.FoxLegacyProperties;
		  assertTrue(foxLegacyProperties.Count == 0);

		  assertNotNull("process engine controller for engine __test is installed ", container.getRequiredService(ServiceNames.forManagedProcessEngine("__test")));

		  // check we have parsed the plugin configurations
		  IList<ProcessEnginePluginXml> pluginConfigurations = metadata.PluginConfigurations;

		  assertEquals(1, pluginConfigurations.Count);

		  ProcessEnginePluginXml processEnginePluginXml = pluginConfigurations[0];
		  assertEquals("org.camunda.bpm.identity.impl.ldap.plugin.LdapIdentityProviderPlugin", processEnginePluginXml.PluginClass);
		  IDictionary<string, string> processEnginePluginXmlProperties = processEnginePluginXml.Properties;
		  assertEquals("abc", processEnginePluginXmlProperties["test"]);
		  assertEquals("123", processEnginePluginXmlProperties["number"]);
		  assertEquals("true", processEnginePluginXmlProperties["bool"]);

		  string persistedSubsystemXml = services.PersistedSubsystemXml;
		  compareXml(null, subsystemXml, persistedSubsystemXml);

		}
		finally
		{
		  foreach (string key in EXPRESSION_PROPERTIES.Keys)
		  {
			System.clearProperty(key);
		  }
		}
	  }

	  private void assertExpressionType(ModelNode operation, params string[] elements)
	  {
		assertModelType(ModelType.EXPRESSION, operation, elements);
	  }

	  private void assertStringType(ModelNode operation, params string[] elements)
	  {
		assertModelType(ModelType.STRING, operation, elements);
	  }

	  private void assertModelType(ModelType type, ModelNode operation, params string[] elements)
	  {
		foreach (string element in elements)
		{
		  assertEquals(type, operation.get(element).Type);
		}
	  }
	}

}