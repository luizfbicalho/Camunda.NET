using System.Collections.Generic;
using System.IO;

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
namespace org.camunda.bpm.engine.rest
{

	using Header = io.restassured.http.Header;
	using ContentType = org.apache.http.entity.ContentType;
	using ActivityInstanceImpl = org.camunda.bpm.engine.impl.persistence.entity.ActivityInstanceImpl;
	using TransitionInstanceImpl = org.camunda.bpm.engine.impl.persistence.entity.TransitionInstanceImpl;
	using Hal = org.camunda.bpm.engine.rest.hal.Hal;
	using ProcessEngineProvider = org.camunda.bpm.engine.rest.spi.ProcessEngineProvider;
	using MockedProcessEngineProvider = org.camunda.bpm.engine.rest.spi.impl.MockedProcessEngineProvider;
	using ActivityInstance = org.camunda.bpm.engine.runtime.ActivityInstance;
	using TransitionInstance = org.camunda.bpm.engine.runtime.TransitionInstance;
	using VariableMap = org.camunda.bpm.engine.variable.VariableMap;
	using Variables = org.camunda.bpm.engine.variable.Variables;
	using BytesValue = org.camunda.bpm.engine.variable.value.BytesValue;
	using TypedValue = org.camunda.bpm.engine.variable.value.TypedValue;
	using BeforeClass = org.junit.BeforeClass;

	using RestAssured = io.restassured.RestAssured;

	public abstract class AbstractRestServiceTest
	{

	  protected internal static ProcessEngine processEngine;
	  protected internal const string TEST_RESOURCE_ROOT_PATH = "/rest-test";
	  protected internal static int PORT;

	  protected internal static readonly Header ACCEPT_WILDCARD_HEADER = new Header("Accept", MediaType.WILDCARD);
	  protected internal static readonly Header ACCEPT_JSON_HEADER = new Header("Accept", MediaType.APPLICATION_JSON);
	  protected internal static readonly Header ACCEPT_HAL_HEADER = new Header("Accept", Hal.APPLICATION_HAL_JSON);

	  protected internal static readonly string POST_JSON_CONTENT_TYPE = ContentType.create(MediaType.APPLICATION_JSON, "UTF-8").ToString();
	  protected internal static readonly string XHTML_XML_CONTENT_TYPE = ContentType.create(MediaType.APPLICATION_XHTML_XML).ToString();

	  protected internal const string EMPTY_JSON_OBJECT = "{}";

	  private const string PROPERTIES_FILE_PATH = "/testconfig.properties";
	  private const string PORT_PROPERTY = "rest.http.port";

	  protected internal const string EXAMPLE_VARIABLE_KEY = "aVariableKey";
	  protected internal static readonly TypedValue EXAMPLE_VARIABLE_VALUE = Variables.stringValue("aVariableValue");
	  protected internal const string EXAMPLE_BYTES_VARIABLE_KEY = "aBytesVariableKey";
	  protected internal static readonly BytesValue EXAMPLE_VARIABLE_VALUE_BYTES = Variables.byteArrayValue("someBytes".Bytes);
	  protected internal const string EXAMPLE_ANOTHER_VARIABLE_KEY = "anotherVariableKey";

	  protected internal static readonly VariableMap EXAMPLE_VARIABLES = Variables.createVariables();
	  static AbstractRestServiceTest()
	  {
		EXAMPLE_VARIABLES.putValueTyped(EXAMPLE_VARIABLE_KEY, EXAMPLE_VARIABLE_VALUE);
		EXAMPLE_VARIABLES_WITH_NULL_VALUE.putValueTyped(EXAMPLE_ANOTHER_VARIABLE_KEY, Variables.untypedNullValue());
		ActivityInstanceImpl instance = (ActivityInstanceImpl) EXAMPLE_ACTIVITY_INSTANCE;
		instance.Id = EXAMPLE_ACTIVITY_INSTANCE_ID;
		instance.ParentActivityInstanceId = EXAMPLE_PARENT_ACTIVITY_INSTANCE_ID;
		instance.ActivityId = EXAMPLE_ACTIVITY_ID;
		instance.ActivityType = CHILD_EXAMPLE_ACTIVITY_TYPE;
		instance.ActivityName = EXAMPLE_ACTIVITY_NAME;
		instance.ProcessInstanceId = EXAMPLE_PROCESS_INSTANCE_ID;
		instance.ProcessDefinitionId = EXAMPLE_PROCESS_DEFINITION_ID;
		instance.BusinessKey = EXAMPLE_BUSINESS_KEY;
		instance.ExecutionIds = new string[]{EXAMPLE_EXECUTION_ID};

		ActivityInstanceImpl childActivity = new ActivityInstanceImpl();
		childActivity.Id = CHILD_EXAMPLE_ACTIVITY_INSTANCE_ID;
		childActivity.ParentActivityInstanceId = CHILD_EXAMPLE_PARENT_ACTIVITY_INSTANCE_ID;
		childActivity.ActivityId = CHILD_EXAMPLE_ACTIVITY_ID;
		childActivity.ActivityName = CHILD_EXAMPLE_ACTIVITY_NAME;
		childActivity.ActivityType = CHILD_EXAMPLE_ACTIVITY_TYPE;
		childActivity.ProcessInstanceId = CHILD_EXAMPLE_PROCESS_INSTANCE_ID;
		childActivity.ProcessDefinitionId = CHILD_EXAMPLE_PROCESS_DEFINITION_ID;
		childActivity.BusinessKey = CHILD_EXAMPLE_BUSINESS_KEY;
		childActivity.ExecutionIds = new string[]{EXAMPLE_EXECUTION_ID};
		childActivity.ChildActivityInstances = new ActivityInstance[0];
		childActivity.ChildTransitionInstances = new TransitionInstance[0];

		TransitionInstanceImpl childTransition = new TransitionInstanceImpl();
		childTransition.Id = CHILD_EXAMPLE_ACTIVITY_INSTANCE_ID;
		childTransition.ParentActivityInstanceId = CHILD_EXAMPLE_PARENT_ACTIVITY_INSTANCE_ID;
		childTransition.ActivityId = CHILD_EXAMPLE_ACTIVITY_ID;
		childTransition.ActivityName = CHILD_EXAMPLE_ACTIVITY_NAME;
		childTransition.ActivityType = CHILD_EXAMPLE_ACTIVITY_TYPE;
		childTransition.ProcessInstanceId = CHILD_EXAMPLE_PROCESS_INSTANCE_ID;
		childTransition.ProcessDefinitionId = CHILD_EXAMPLE_PROCESS_DEFINITION_ID;
		childTransition.ExecutionId = EXAMPLE_EXECUTION_ID;

		instance.ChildActivityInstances = new ActivityInstance[]{childActivity};
		instance.ChildTransitionInstances = new TransitionInstance[]{childTransition};
	  }

	  protected internal static readonly VariableMap EXAMPLE_VARIABLES_WITH_NULL_VALUE = Variables.createVariables();

	  protected internal const string EXAMPLE_ACTIVITY_INSTANCE_ID = "anActivityInstanceId";
	  protected internal const string EXAMPLE_PARENT_ACTIVITY_INSTANCE_ID = "aParentActivityInstanceId";
	  protected internal const string EXAMPLE_ACTIVITY_ID = "anActivityId";
	  protected internal const string EXAMPLE_ACTIVITY_NAME = "anActivityName";
	  protected internal const string EXAMPLE_PROCESS_INSTANCE_ID = "aProcessInstanceId";
	  protected internal const string EXAMPLE_PROCESS_DEFINITION_ID = "aProcessDefinitionId";
	  protected internal const string EXAMPLE_PROCESS_DEFINITION_KEY = "aKey";
	  protected internal const string EXAMPLE_BUSINESS_KEY = "aBusinessKey";
	  protected internal const string EXAMPLE_EXECUTION_ID = "anExecutionId";

	  protected internal const string CHILD_EXAMPLE_ACTIVITY_INSTANCE_ID = "aChildActivityInstanceId";
	  protected internal const string CHILD_EXAMPLE_PARENT_ACTIVITY_INSTANCE_ID = "aChildParentActivityInstanceId";
	  protected internal const string CHILD_EXAMPLE_ACTIVITY_ID = "aChildActivityId";
	  protected internal const string CHILD_EXAMPLE_ACTIVITY_TYPE = "aChildActivityType";
	  protected internal const string CHILD_EXAMPLE_ACTIVITY_NAME = "aChildActivityName";
	  protected internal const string CHILD_EXAMPLE_PROCESS_INSTANCE_ID = "aChildProcessInstanceId";
	  protected internal const string CHILD_EXAMPLE_PROCESS_DEFINITION_ID = "aChildProcessDefinitionId";
	  protected internal const string CHILD_EXAMPLE_BUSINESS_KEY = "aChildBusinessKey";

	  protected internal static readonly ActivityInstance EXAMPLE_ACTIVITY_INSTANCE = new ActivityInstanceImpl();


	  private static Properties connectionProperties = null;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @BeforeClass public static void setUp() throws java.io.IOException
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public static void setUp()
	  {
		setupTestScenario();
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected static void setupTestScenario() throws java.io.IOException
	  protected internal static void setupTestScenario()
	  {
		setupRestAssured();

		ServiceLoader<ProcessEngineProvider> serviceLoader = ServiceLoader.load(typeof(ProcessEngineProvider));
		IEnumerator<ProcessEngineProvider> iterator = serviceLoader.GetEnumerator();

//JAVA TO C# CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
		if (iterator.hasNext())
		{
//JAVA TO C# CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
		  MockedProcessEngineProvider provider = (MockedProcessEngineProvider) iterator.next();

		  // reset engine mocks before every test
		  provider.resetEngines();

		  processEngine = provider.DefaultProcessEngine;
		}
	  }

	  protected internal virtual ProcessEngine getProcessEngine(string name)
	  {
		ServiceLoader<ProcessEngineProvider> serviceLoader = ServiceLoader.load(typeof(ProcessEngineProvider));
		IEnumerator<ProcessEngineProvider> iterator = serviceLoader.GetEnumerator();

//JAVA TO C# CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
		if (iterator.hasNext())
		{
//JAVA TO C# CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
		  ProcessEngineProvider provider = iterator.next();
		  return provider.getProcessEngine(name);
		}
		else
		{
		  throw new ProcessEngineException("No provider found");
		}
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private static void setupRestAssured() throws java.io.IOException
	  private static void setupRestAssured()
	  {
		if (connectionProperties == null)
		{
		  Stream propStream = null;
		  try
		  {
			propStream = typeof(AbstractRestServiceTest).getResourceAsStream(PROPERTIES_FILE_PATH);
			connectionProperties = new Properties();
			connectionProperties.load(propStream);
		  }
		  finally
		  {
			propStream.Close();
		  }
		}

		PORT = int.Parse(connectionProperties.getProperty(PORT_PROPERTY));
		RestAssured.port = PORT;
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected java.io.File getFile(String path) throws java.net.URISyntaxException
	  protected internal virtual File getFile(string path)
	  {
		URI uri = this.GetType().getResource(path).toURI();
		return new File(uri);
	  }

	}

}