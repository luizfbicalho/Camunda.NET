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
namespace org.camunda.bpm.engine.impl.dmn.configuration
{
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.util.EnsureUtil.ensureNotNull;

	using DmnDecisionEvaluationListener = org.camunda.bpm.dmn.engine.@delegate.DmnDecisionEvaluationListener;
	using DefaultDmnEngineConfiguration = org.camunda.bpm.dmn.engine.impl.DefaultDmnEngineConfiguration;
	using DmnScriptEngineResolver = org.camunda.bpm.dmn.engine.impl.spi.el.DmnScriptEngineResolver;
	using DmnTransformer = org.camunda.bpm.dmn.engine.impl.spi.transform.DmnTransformer;
	using ProcessEngineElProvider = org.camunda.bpm.engine.impl.dmn.el.ProcessEngineElProvider;
	using DecisionDefinitionHandler = org.camunda.bpm.engine.impl.dmn.transformer.DecisionDefinitionHandler;
	using DecisionRequirementsDefinitionTransformHandler = org.camunda.bpm.engine.impl.dmn.transformer.DecisionRequirementsDefinitionTransformHandler;
	using ExpressionManager = org.camunda.bpm.engine.impl.el.ExpressionManager;
	using HistoryLevel = org.camunda.bpm.engine.impl.history.HistoryLevel;
	using HistoryDecisionEvaluationListener = org.camunda.bpm.engine.impl.history.parser.HistoryDecisionEvaluationListener;
	using DmnHistoryEventProducer = org.camunda.bpm.engine.impl.history.producer.DmnHistoryEventProducer;
	using MetricsDecisionEvaluationListener = org.camunda.bpm.engine.impl.metrics.dmn.MetricsDecisionEvaluationListener;
	using Decision = org.camunda.bpm.model.dmn.instance.Decision;
	using Definitions = org.camunda.bpm.model.dmn.instance.Definitions;

	/// <summary>
	/// Modify the given DMN engine configuration so that the DMN engine can be used
	/// from the process engine. Note that properties will not be overridden if they
	/// are set on the configuration, except the transform handler for the decision table.
	/// 
	/// @author Philipp Ossler
	/// </summary>
	public class DmnEngineConfigurationBuilder
	{

	  protected internal readonly DefaultDmnEngineConfiguration dmnEngineConfiguration;

//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal HistoryLevel historyLevel_Conflict;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal DmnHistoryEventProducer dmnHistoryEventProducer_Conflict;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal DmnScriptEngineResolver scriptEngineResolver_Conflict;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal ExpressionManager expressionManager_Conflict;

	  /// <summary>
	  /// Creates a new builder to modify the given DMN engine configuration.
	  /// </summary>
	  public DmnEngineConfigurationBuilder(DefaultDmnEngineConfiguration dmnEngineConfiguration)
	  {
		ensureNotNull("dmnEngineConfiguration", dmnEngineConfiguration);

		this.dmnEngineConfiguration = dmnEngineConfiguration;
	  }

	  public virtual DmnEngineConfigurationBuilder historyLevel(HistoryLevel historyLevel)
	  {
		this.historyLevel_Conflict = historyLevel;

		return this;
	  }

	  public virtual DmnEngineConfigurationBuilder dmnHistoryEventProducer(DmnHistoryEventProducer dmnHistoryEventProducer)
	  {
		this.dmnHistoryEventProducer_Conflict = dmnHistoryEventProducer;

		return this;
	  }

	  public virtual DmnEngineConfigurationBuilder scriptEngineResolver(DmnScriptEngineResolver scriptEngineResolver)
	  {
		this.scriptEngineResolver_Conflict = scriptEngineResolver;

		return this;
	  }

	  public virtual DmnEngineConfigurationBuilder expressionManager(ExpressionManager expressionManager)
	  {
		this.expressionManager_Conflict = expressionManager;

		return this;
	  }

	  /// <summary>
	  /// Modify the given DMN engine configuration and return it.
	  /// </summary>
	  public virtual DefaultDmnEngineConfiguration build()
	  {

		IList<DmnDecisionEvaluationListener> decisionEvaluationListeners = createCustomPostDecisionEvaluationListeners();
		dmnEngineConfiguration.CustomPostDecisionEvaluationListeners = decisionEvaluationListeners;

		// override the decision table handler
		DmnTransformer dmnTransformer = dmnEngineConfiguration.Transformer;
		dmnTransformer.ElementTransformHandlerRegistry.addHandler(typeof(Definitions), new DecisionRequirementsDefinitionTransformHandler());
		dmnTransformer.ElementTransformHandlerRegistry.addHandler(typeof(Decision), new DecisionDefinitionHandler());

		// do not override the script engine resolver if set
		if (dmnEngineConfiguration.ScriptEngineResolver == null)
		{
		  ensureNotNull("scriptEngineResolver", scriptEngineResolver_Conflict);

		  dmnEngineConfiguration.ScriptEngineResolver = scriptEngineResolver_Conflict;
		}

		// do not override the el provider if set
		if (dmnEngineConfiguration.ElProvider == null)
		{
		  ensureNotNull("expressionManager", expressionManager_Conflict);

		  ProcessEngineElProvider elProvider = new ProcessEngineElProvider(expressionManager_Conflict);
		  dmnEngineConfiguration.ElProvider = elProvider;
		}

		return dmnEngineConfiguration;
	  }

	  protected internal virtual IList<DmnDecisionEvaluationListener> createCustomPostDecisionEvaluationListeners()
	  {
		ensureNotNull("dmnHistoryEventProducer", dmnHistoryEventProducer_Conflict);
		// note that the history level may be null - see CAM-5165

		HistoryDecisionEvaluationListener historyDecisionEvaluationListener = new HistoryDecisionEvaluationListener(dmnHistoryEventProducer_Conflict, historyLevel_Conflict);

		IList<DmnDecisionEvaluationListener> customPostDecisionEvaluationListeners = dmnEngineConfiguration.CustomPostDecisionEvaluationListeners;
		customPostDecisionEvaluationListeners.Add(new MetricsDecisionEvaluationListener());
		customPostDecisionEvaluationListeners.Add(historyDecisionEvaluationListener);

		return customPostDecisionEvaluationListeners;
	  }

	}

}