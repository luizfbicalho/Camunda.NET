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
	using ProcessEngineConfigurationImpl = org.camunda.bpm.engine.impl.cfg.ProcessEngineConfigurationImpl;
	using CommandContextFunctionMapper = org.camunda.bpm.engine.impl.el.CommandContextFunctionMapper;
	using DateTimeFunctionMapper = org.camunda.bpm.engine.impl.el.DateTimeFunctionMapper;
	using FunctionMapper = org.camunda.bpm.engine.impl.javax.el.FunctionMapper;
	using After = org.junit.After;
	using Assert = org.junit.Assert;
	using Test = org.junit.Test;

	/// <summary>
	/// @author Thorben Lindhauer
	/// </summary>
	public class CustomExpressionManagerTest
	{

	  protected internal ProcessEngine engine;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testBuiltinFunctionMapperRegistration()
	  public virtual void testBuiltinFunctionMapperRegistration()
	  {
		// given a process engine configuration with a custom function mapper
		ProcessEngineConfigurationImpl config = (ProcessEngineConfigurationImpl) ProcessEngineConfiguration.createStandaloneInMemProcessEngineConfiguration().setJdbcUrl("jdbc:h2:mem:camunda" + this.GetType().Name);

		CustomExpressionManager customExpressionManager = new CustomExpressionManager();
		Assert.assertTrue(customExpressionManager.FunctionMappers.Count == 0);
		config.ExpressionManager = customExpressionManager;

		// when the engine is initialized
		engine = config.buildProcessEngine();

		// then two default function mappers should be registered
		Assert.assertSame(customExpressionManager, config.ExpressionManager);
		Assert.assertEquals(2, customExpressionManager.FunctionMappers.Count);

		bool commandContextMapperFound = false;
		bool dateTimeMapperFound = false;

		foreach (FunctionMapper functionMapper in customExpressionManager.FunctionMappers)
		{
		  if (functionMapper is CommandContextFunctionMapper)
		  {
			commandContextMapperFound = true;
		  }

		  if (functionMapper is DateTimeFunctionMapper)
		  {
			dateTimeMapperFound = true;
		  }
		}

		Assert.assertTrue(commandContextMapperFound && dateTimeMapperFound);
	  }

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
	}

}