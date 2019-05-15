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
namespace org.camunda.bpm.integrationtest.functional.scriptengine
{
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertNotEquals;

	using AbstractProcessApplication = org.camunda.bpm.application.AbstractProcessApplication;
	using AbstractScriptEngineFactory = org.camunda.bpm.integrationtest.functional.scriptengine.engine.AbstractScriptEngineFactory;
	using DummyScriptEngineFactory = org.camunda.bpm.integrationtest.functional.scriptengine.engine.DummyScriptEngineFactory;
	using Deployment = org.jboss.arquillian.container.test.api.Deployment;
	using StringAsset = org.jboss.shrinkwrap.api.asset.StringAsset;
	using WebArchive = org.jboss.shrinkwrap.api.spec.WebArchive;
	using Test = org.junit.Test;

	/// <summary>
	/// @author Roman Smirnov
	/// 
	/// </summary>
	public class PaLocalScriptEngineDisabledCacheTest : AbstractPaLocalScriptEngineTest
	{

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public static org.jboss.shrinkwrap.api.spec.WebArchive createProcessApplication()
	  public static WebArchive createProcessApplication()
	  {
		return initWebArchiveDeployment().addClass(typeof(AbstractPaLocalScriptEngineTest)).addClass(typeof(AbstractScriptEngineFactory)).addClass(typeof(DummyScriptEngineFactory)).addAsResource(new StringAsset(DUMMY_SCRIPT_ENGINE_FACTORY_SPI), SCRIPT_ENGINE_FACTORY_PATH).addAsResource(createScriptTaskProcess(SCRIPT_FORMAT, SCRIPT_TEXT), "process.bpmn20.xml");
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldNotCacheScriptEngine()
	  public virtual void shouldNotCacheScriptEngine()
	  {
		AbstractProcessApplication processApplication = (AbstractProcessApplication) ProcessApplication;
		assertNotEquals(processApplication.getScriptEngineForName(SCRIPT_FORMAT, false), processApplication.getScriptEngineForName(SCRIPT_FORMAT, false));
	  }

	}

}