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
namespace org.camunda.bpm.engine.impl.bpmn.parser
{

	using BpmnParseFactory = org.camunda.bpm.engine.impl.cfg.BpmnParseFactory;
	using ExpressionManager = org.camunda.bpm.engine.impl.el.ExpressionManager;
	using Parser = org.camunda.bpm.engine.impl.util.xml.Parser;


	/// <summary>
	/// Parser for BPMN 2.0 process models.
	/// 
	/// There is only one instance of this parser in the process engine.
	/// This <seealso cref="Parser"/> creates <seealso cref="BpmnParse"/> instances that
	/// can be used to actually parse the BPMN 2.0 XML process definitions.
	/// 
	/// @author Tom Baeyens
	/// @author Joram Barrez
	/// </summary>
	public class BpmnParser : Parser
	{

	  /// <summary>
	  /// The BPMN 2.0 namespace
	  /// </summary>
	  public const string BPMN20_NS = "http://www.omg.org/spec/BPMN/20100524/MODEL";

	  /// <summary>
	  /// The location of the BPMN 2.0 XML schema.
	  /// </summary>
	  public const string BPMN_20_SCHEMA_LOCATION = "org/camunda/bpm/engine/impl/bpmn/parser/BPMN20.xsd";

	  /// <summary>
	  /// The namespace of the camunda custom BPMN extensions.
	  /// </summary>
	  public const string CAMUNDA_BPMN_EXTENSIONS_NS = "http://camunda.org/schema/1.0/bpmn";

	  /// <summary>
	  /// The namespace of the Activiti custom BPMN extensions. </summary>
	  /// @deprecated use <seealso cref="#CAMUNDA_BPMN_EXTENSIONS_NS"/> 
	  [Obsolete("use <seealso cref=\"#CAMUNDA_BPMN_EXTENSIONS_NS\"/>")]
	  public const string ACTIVITI_BPMN_EXTENSIONS_NS = "http://activiti.org/bpmn";

	  /// <summary>
	  /// The namepace of the BPMN 2.0 diagram interchange elements.
	  /// </summary>
	  public const string BPMN_DI_NS = "http://www.omg.org/spec/BPMN/20100524/DI";

	  /// <summary>
	  /// The namespace of the BPMN 2.0 diagram common elements.
	  /// </summary>
	  public const string BPMN_DC_NS = "http://www.omg.org/spec/DD/20100524/DC";

	  /// <summary>
	  /// The namespace of the generic OMG DI elements (don't ask me why they didnt use the BPMN_DI_NS ...)
	  /// </summary>
	  public const string OMG_DI_NS = "http://www.omg.org/spec/DD/20100524/DI";

	  /// <summary>
	  /// The Schema-Instance namespace.
	  /// </summary>
	  public const string XSI_NS = "http://www.w3.org/2001/XMLSchema-instance";

	  protected internal ExpressionManager expressionManager;
	  protected internal IList<BpmnParseListener> parseListeners = new List<BpmnParseListener>();

	  protected internal BpmnParseFactory bpmnParseFactory;

	  public BpmnParser(ExpressionManager expressionManager, BpmnParseFactory bpmnParseFactory)
	  {
		this.expressionManager = expressionManager;
		this.bpmnParseFactory = bpmnParseFactory;
	  }

	  /// <summary>
	  /// Creates a new <seealso cref="BpmnParse"/> instance that can be used
	  /// to parse only one BPMN 2.0 process definition.
	  /// </summary>
	  public override BpmnParse createParse()
	  {
		return bpmnParseFactory.createBpmnParse(this);
	  }

	  public virtual ExpressionManager ExpressionManager
	  {
		  get
		  {
			return expressionManager;
		  }
		  set
		  {
			this.expressionManager = value;
		  }
	  }


	  public virtual IList<BpmnParseListener> ParseListeners
	  {
		  get
		  {
			return parseListeners;
		  }
		  set
		  {
			this.parseListeners = value;
		  }
	  }

	}

}