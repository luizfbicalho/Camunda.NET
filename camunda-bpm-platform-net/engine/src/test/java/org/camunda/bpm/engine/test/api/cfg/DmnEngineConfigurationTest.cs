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
namespace org.camunda.bpm.engine.test.api.cfg
{
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.CoreMatchers.hasItem;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.CoreMatchers.@is;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.CoreMatchers.notNullValue;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertEquals;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertThat;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.mockito.Mockito.mock;


	using DmnEngine = org.camunda.bpm.dmn.engine.DmnEngine;
	using DmnEngineConfiguration = org.camunda.bpm.dmn.engine.DmnEngineConfiguration;
	using DmnDecisionTableEvaluationListener = org.camunda.bpm.dmn.engine.@delegate.DmnDecisionTableEvaluationListener;
	using DefaultDmnEngineConfiguration = org.camunda.bpm.dmn.engine.impl.DefaultDmnEngineConfiguration;
	using DmnScriptEngineResolver = org.camunda.bpm.dmn.engine.impl.spi.el.DmnScriptEngineResolver;
	using ElProvider = org.camunda.bpm.dmn.engine.impl.spi.el.ElProvider;
	using FeelEngineFactory = org.camunda.bpm.dmn.feel.impl.FeelEngineFactory;
	using ProcessEngineConfigurationImpl = org.camunda.bpm.engine.impl.cfg.ProcessEngineConfigurationImpl;
	using ProcessEngineElProvider = org.camunda.bpm.engine.impl.dmn.el.ProcessEngineElProvider;
	using After = org.junit.After;
	using Test = org.junit.Test;

	/// <summary>
	/// @author Philipp Ossler
	/// </summary>
	public class DmnEngineConfigurationTest
	{

	  protected internal const string CONFIGURATION_XML = "org/camunda/bpm/engine/test/api/cfg/custom-dmn-camunda.cfg.xml";

	  protected internal ProcessEngine engine;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @After public void tearDown()
	  public virtual void tearDown()
	  {
		if (engine != null)
		{
		  engine.close();
		  engine = null;
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void setDefaultInputExpressionLanguage()
	  public virtual void setDefaultInputExpressionLanguage()
	  {
		// given a DMN engine configuration with default expression language
		DefaultDmnEngineConfiguration dmnEngineConfiguration = (DefaultDmnEngineConfiguration) DmnEngineConfiguration.createDefaultDmnEngineConfiguration();
		dmnEngineConfiguration.DefaultInputExpressionExpressionLanguage = "groovy";

		ProcessEngineConfigurationImpl processEngineConfiguration = createProcessEngineConfiguration();
		processEngineConfiguration.DmnEngineConfiguration = dmnEngineConfiguration;

		// when the engine is initialized
		engine = processEngineConfiguration.buildProcessEngine();

		// then the default expression language should be set on the DMN engine
		assertThat(ConfigurationOfDmnEngine.DefaultInputExpressionExpressionLanguage, @is("groovy"));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void setCustomPostTableExecutionListener()
	  public virtual void setCustomPostTableExecutionListener()
	  {
		// given a DMN engine configuration with custom listener
		DefaultDmnEngineConfiguration dmnEngineConfiguration = (DefaultDmnEngineConfiguration) DmnEngineConfiguration.createDefaultDmnEngineConfiguration();
		DmnDecisionTableEvaluationListener customEvaluationListener = mock(typeof(DmnDecisionTableEvaluationListener));
		IList<DmnDecisionTableEvaluationListener> customListeners = new List<DmnDecisionTableEvaluationListener>();
		customListeners.Add(customEvaluationListener);
		dmnEngineConfiguration.CustomPostDecisionTableEvaluationListeners = customListeners;

		ProcessEngineConfigurationImpl processEngineConfiguration = createProcessEngineConfiguration();
		processEngineConfiguration.DmnEngineConfiguration = dmnEngineConfiguration;

		// when the engine is initialized
		engine = processEngineConfiguration.buildProcessEngine();

		// then the custom listener should be set on the DMN engine
		assertThat(ConfigurationOfDmnEngine.CustomPostDecisionTableEvaluationListeners, hasItem(customEvaluationListener));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void setFeelEngineFactory()
	  public virtual void setFeelEngineFactory()
	  {
		// given a DMN engine configuration with feel engine factory
		DefaultDmnEngineConfiguration dmnEngineConfiguration = (DefaultDmnEngineConfiguration) DmnEngineConfiguration.createDefaultDmnEngineConfiguration();
		FeelEngineFactory feelEngineFactory = mock(typeof(FeelEngineFactory));
		dmnEngineConfiguration.FeelEngineFactory = feelEngineFactory;

		ProcessEngineConfigurationImpl processEngineConfiguration = createProcessEngineConfiguration();
		processEngineConfiguration.DmnEngineConfiguration = dmnEngineConfiguration;

		// when the engine is initialized
		engine = processEngineConfiguration.buildProcessEngine();

		// then the feel engine factory should be set on the DMN engine
		assertThat(ConfigurationOfDmnEngine.FeelEngineFactory, @is(feelEngineFactory));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void setScriptEngineResolver()
	  public virtual void setScriptEngineResolver()
	  {
		// given a DMN engine configuration with script engine resolver
		DefaultDmnEngineConfiguration dmnEngineConfiguration = (DefaultDmnEngineConfiguration) DmnEngineConfiguration.createDefaultDmnEngineConfiguration();
		DmnScriptEngineResolver scriptEngineResolver = mock(typeof(DmnScriptEngineResolver));
		dmnEngineConfiguration.ScriptEngineResolver = scriptEngineResolver;

		ProcessEngineConfigurationImpl processEngineConfiguration = createProcessEngineConfiguration();
		processEngineConfiguration.DmnEngineConfiguration = dmnEngineConfiguration;

		// when the engine is initialized
		engine = processEngineConfiguration.buildProcessEngine();

		// then the script engine resolver should be set on the DMN engine
		assertThat(ConfigurationOfDmnEngine.ScriptEngineResolver, @is(scriptEngineResolver));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void setElProvider()
	  public virtual void setElProvider()
	  {
		// given a DMN engine configuration with el provider
		DefaultDmnEngineConfiguration dmnEngineConfiguration = (DefaultDmnEngineConfiguration) DmnEngineConfiguration.createDefaultDmnEngineConfiguration();
		ElProvider elProvider = mock(typeof(ElProvider));
		dmnEngineConfiguration.ElProvider = elProvider;

		ProcessEngineConfigurationImpl processEngineConfiguration = createProcessEngineConfiguration();
		processEngineConfiguration.DmnEngineConfiguration = dmnEngineConfiguration;

		// when the engine is initialized
		engine = processEngineConfiguration.buildProcessEngine();

		// then the el provider should be set on the DMN engine
		assertThat(ConfigurationOfDmnEngine.ElProvider, @is(elProvider));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void setProcessEngineElProviderByDefault()
	  public virtual void setProcessEngineElProviderByDefault()
	  {
		// given a default DMN engine configuration without el provider
		ProcessEngineConfigurationImpl processEngineConfiguration = createProcessEngineConfiguration();

		// when the engine is initialized
		engine = processEngineConfiguration.buildProcessEngine();

		// then the DMN engine should use the process engine el provider
		assertEquals(typeof(ProcessEngineElProvider), ConfigurationOfDmnEngine.ElProvider.GetType());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void setProcessEngineScriptEnginesByDefault()
	  public virtual void setProcessEngineScriptEnginesByDefault()
	  {
		// given a default DMN engine configuration without script engine resolver
		ProcessEngineConfigurationImpl processEngineConfiguration = createProcessEngineConfiguration();

		// when the engine is initialized
		engine = processEngineConfiguration.buildProcessEngine();

		// then the DMN engine should use the script engines from the process engine
		assertEquals(processEngineConfiguration.ScriptingEngines, ConfigurationOfDmnEngine.ScriptEngineResolver);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void setDmnEngineConfigurationOverXmlConfiguration()
	  public virtual void setDmnEngineConfigurationOverXmlConfiguration()
	  {
		// given an embedded DMN engine configuration in XML process engine configuration
		// with default expression language
		ProcessEngineConfigurationImpl processEngineConfiguration = (ProcessEngineConfigurationImpl) ProcessEngineConfiguration.createProcessEngineConfigurationFromResource(CONFIGURATION_XML);

		// checks that the configuration is set as on XML
		DefaultDmnEngineConfiguration dmnEngineConfiguration = processEngineConfiguration.DmnEngineConfiguration;
		assertThat(dmnEngineConfiguration, @is(notNullValue()));
		assertThat(dmnEngineConfiguration.DefaultInputExpressionExpressionLanguage, @is("groovy"));

		// when the engine is initialized
		engine = processEngineConfiguration.buildProcessEngine();

		// then the default expression language should be set in the DMN engine
		assertThat(ConfigurationOfDmnEngine.DefaultInputExpressionExpressionLanguage, @is("groovy"));
	  }

	  protected internal virtual ProcessEngineConfigurationImpl createProcessEngineConfiguration()
	  {
		return (ProcessEngineConfigurationImpl) ProcessEngineConfiguration.createStandaloneInMemProcessEngineConfiguration().setJdbcUrl("jdbc:h2:mem:camunda" + this.GetType().Name);
	  }

	  protected internal virtual DefaultDmnEngineConfiguration ConfigurationOfDmnEngine
	  {
		  get
		  {
			ProcessEngineConfigurationImpl processEngineConfiguration = (ProcessEngineConfigurationImpl) engine.ProcessEngineConfiguration;
    
			DmnEngine dmnEngine = processEngineConfiguration.DmnEngine;
			return (DefaultDmnEngineConfiguration) dmnEngine.Configuration;
		  }
	  }

	}

}