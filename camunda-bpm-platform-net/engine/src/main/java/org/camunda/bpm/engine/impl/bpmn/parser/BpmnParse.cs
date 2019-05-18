using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
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
namespace org.camunda.bpm.engine.impl.bpmn.parser
{
	using ExecutionListener = org.camunda.bpm.engine.@delegate.ExecutionListener;
	using TaskListener = org.camunda.bpm.engine.@delegate.TaskListener;
	using VariableListener = org.camunda.bpm.engine.@delegate.VariableListener;
	using org.camunda.bpm.engine.impl.bpmn.behavior;
	using BpmnProperties = org.camunda.bpm.engine.impl.bpmn.helper.BpmnProperties;
	using ClassDelegateExecutionListener = org.camunda.bpm.engine.impl.bpmn.listener.ClassDelegateExecutionListener;
	using DelegateExpressionExecutionListener = org.camunda.bpm.engine.impl.bpmn.listener.DelegateExpressionExecutionListener;
	using ExpressionExecutionListener = org.camunda.bpm.engine.impl.bpmn.listener.ExpressionExecutionListener;
	using ScriptExecutionListener = org.camunda.bpm.engine.impl.bpmn.listener.ScriptExecutionListener;
	using Context = org.camunda.bpm.engine.impl.context.Context;
	using org.camunda.bpm.engine.impl.core.model;
	using CallableElementBinding = org.camunda.bpm.engine.impl.core.model.BaseCallableElement.CallableElementBinding;
	using IoMapping = org.camunda.bpm.engine.impl.core.variable.mapping.IoMapping;
	using ConstantValueProvider = org.camunda.bpm.engine.impl.core.variable.mapping.value.ConstantValueProvider;
	using NullValueProvider = org.camunda.bpm.engine.impl.core.variable.mapping.value.NullValueProvider;
	using ParameterValueProvider = org.camunda.bpm.engine.impl.core.variable.mapping.value.ParameterValueProvider;
	using DecisionResultMapper = org.camunda.bpm.engine.impl.dmn.result.DecisionResultMapper;
	using org.camunda.bpm.engine.impl.el;
	using EventType = org.camunda.bpm.engine.impl.@event.EventType;
	using org.camunda.bpm.engine.impl.form.handler;
	using org.camunda.bpm.engine.impl.jobexecutor;
	using DeploymentEntity = org.camunda.bpm.engine.impl.persistence.entity.DeploymentEntity;
	using JobEntity = org.camunda.bpm.engine.impl.persistence.entity.JobEntity;
	using ProcessDefinitionEntity = org.camunda.bpm.engine.impl.persistence.entity.ProcessDefinitionEntity;
	using PvmTransition = org.camunda.bpm.engine.impl.pvm.PvmTransition;
	using ActivityBehavior = org.camunda.bpm.engine.impl.pvm.@delegate.ActivityBehavior;
	using org.camunda.bpm.engine.impl.pvm.process;
	using LegacyBehavior = org.camunda.bpm.engine.impl.pvm.runtime.LegacyBehavior;
	using ExecutableScript = org.camunda.bpm.engine.impl.scripting.ExecutableScript;
	using ScriptCondition = org.camunda.bpm.engine.impl.scripting.ScriptCondition;
	using ScriptingEngines = org.camunda.bpm.engine.impl.scripting.engine.ScriptingEngines;
	using TaskDecorator = org.camunda.bpm.engine.impl.task.TaskDecorator;
	using TaskDefinition = org.camunda.bpm.engine.impl.task.TaskDefinition;
	using ClassDelegateTaskListener = org.camunda.bpm.engine.impl.task.listener.ClassDelegateTaskListener;
	using DelegateExpressionTaskListener = org.camunda.bpm.engine.impl.task.listener.DelegateExpressionTaskListener;
	using ExpressionTaskListener = org.camunda.bpm.engine.impl.task.listener.ExpressionTaskListener;
	using ScriptTaskListener = org.camunda.bpm.engine.impl.task.listener.ScriptTaskListener;
	using DecisionEvaluationUtil = org.camunda.bpm.engine.impl.util.DecisionEvaluationUtil;
	using ParseUtil = org.camunda.bpm.engine.impl.util.ParseUtil;
	using ReflectUtil = org.camunda.bpm.engine.impl.util.ReflectUtil;
	using ScriptUtil = org.camunda.bpm.engine.impl.util.ScriptUtil;
	using StringUtil = org.camunda.bpm.engine.impl.util.StringUtil;
	using Element = org.camunda.bpm.engine.impl.util.xml.Element;
	using Namespace = org.camunda.bpm.engine.impl.util.xml.Namespace;
	using Parse = org.camunda.bpm.engine.impl.util.xml.Parse;
	using VariableDeclaration = org.camunda.bpm.engine.impl.variable.VariableDeclaration;
	using ProcessDefinition = org.camunda.bpm.engine.repository.ProcessDefinition;


	using static org.camunda.bpm.engine.impl.bpmn.parser.BpmnParseUtil;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.util.ClassDelegateUtil.instantiateDelegate;

	/// <summary>
	/// Specific parsing of one BPMN 2.0 XML file, created by the <seealso cref="BpmnParser"/>.
	/// 
	/// Instances of this class should not be reused and are also not threadsafe.
	/// 
	/// @author Tom Baeyens
	/// @author Bernd Ruecker
	/// @author Joram Barrez
	/// @author Christian Stettler
	/// @author Frederik Heremans
	/// @author Falko Menge
	/// @author Esteban Robles
	/// @author Daniel Meyer
	/// @author Saeid Mirzaei
	/// @author Nico Rehwaldt
	/// @author Ronny Bräunlich
	/// @author Christopher Zell
	/// @author Deivarayan Azhagappan
	/// @author Ingo Richtsmeier
	/// </summary>
	public class BpmnParse : Parse
	{

	  public const string MULTI_INSTANCE_BODY_ID_SUFFIX = "#multiInstanceBody";

	  protected internal static readonly BpmnParseLogger LOG = ProcessEngineLogger.BPMN_PARSE_LOGGER;

	  public const string PROPERTYNAME_DOCUMENTATION = "documentation";
	  public const string PROPERTYNAME_INITIATOR_VARIABLE_NAME = "initiatorVariableName";
	  public const string PROPERTYNAME_HAS_CONDITIONAL_EVENTS = "hasConditionalEvents";
	  public const string PROPERTYNAME_CONDITION = "condition";
	  public const string PROPERTYNAME_CONDITION_TEXT = "conditionText";
	  public const string PROPERTYNAME_VARIABLE_DECLARATIONS = "variableDeclarations";
	  public const string PROPERTYNAME_TIMER_DECLARATION = "timerDeclarations";
	  public const string PROPERTYNAME_MESSAGE_JOB_DECLARATION = "messageJobDeclaration";
	  public const string PROPERTYNAME_ISEXPANDED = "isExpanded";
	  public const string PROPERTYNAME_START_TIMER = "timerStart";
	  public const string PROPERTYNAME_COMPENSATION_HANDLER_ID = "compensationHandler";
	  public const string PROPERTYNAME_IS_FOR_COMPENSATION = "isForCompensation";
	  public const string PROPERTYNAME_EVENT_SUBSCRIPTION_JOB_DECLARATION = "eventJobDeclarations";
	  public const string PROPERTYNAME_THROWS_COMPENSATION = "throwsCompensation";
	  public const string PROPERTYNAME_CONSUMES_COMPENSATION = "consumesCompensation";
	  public const string PROPERTYNAME_JOB_PRIORITY = "jobPriority";
	  public const string PROPERTYNAME_TASK_PRIORITY = "taskPriority";
	  public const string PROPERTYNAME_EXTERNAL_TASK_TOPIC = "topic";
	  public const string PROPERTYNAME_CLASS = "class";
	  public const string PROPERTYNAME_EXPRESSION = "expression";
	  public const string PROPERTYNAME_DELEGATE_EXPRESSION = "delegateExpression";
	  public const string PROPERTYNAME_VARIABLE_MAPPING_CLASS = "variableMappingClass";
	  public const string PROPERTYNAME_VARIABLE_MAPPING_DELEGATE_EXPRESSION = "variableMappingDelegateExpression";
	  public const string PROPERTYNAME_RESOURCE = "resource";
	  public const string PROPERTYNAME_LANGUAGE = "language";
	  public const string TYPE = "type";

	  public const string TRUE = "true";
	  public const string INTERRUPTING = "isInterrupting";

	  public const string CONDITIONAL_EVENT_DEFINITION = "conditionalEventDefinition";
	  public const string ESCALATION_EVENT_DEFINITION = "escalationEventDefinition";
	  public const string COMPENSATE_EVENT_DEFINITION = "compensateEventDefinition";
	  public const string TIMER_EVENT_DEFINITION = "timerEventDefinition";
	  public const string SIGNAL_EVENT_DEFINITION = "signalEventDefinition";
	  public const string MESSAGE_EVENT_DEFINITION = "messageEventDefinition";
	  public const string ERROR_EVENT_DEFINITION = "errorEventDefinition";
	  public const string CANCEL_EVENT_DEFINITION = "cancelEventDefinition";
	  public const string LINK_EVENT_DEFINITION = "linkEventDefinition";
	  public const string CONDITION_EXPRESSION = "conditionExpression";
	  public const string CONDITION = "condition";

	  public static readonly IList<string> VARIABLE_EVENTS = new IList<string> {org.camunda.bpm.engine.@delegate.VariableListener_Fields.CREATE, org.camunda.bpm.engine.@delegate.VariableListener_Fields.DELETE, org.camunda.bpm.engine.@delegate.VariableListener_Fields.UPDATE};

	  /// @deprecated use <seealso cref="BpmnProperties#TYPE"/> 
	  [Obsolete("use <seealso cref=\"BpmnProperties#TYPE\"/>")]
	  public static readonly string PROPERTYNAME_TYPE = BpmnProperties.TYPE.Name;

	  /// @deprecated use <seealso cref="BpmnProperties#ERROR_EVENT_DEFINITIONS"/> 
	  [Obsolete("use <seealso cref=\"BpmnProperties#ERROR_EVENT_DEFINITIONS\"/>")]
	  public static readonly string PROPERTYNAME_ERROR_EVENT_DEFINITIONS = BpmnProperties.ERROR_EVENT_DEFINITIONS.Name;

	  /* process start authorization specific finals */
	  protected internal const string POTENTIAL_STARTER = "potentialStarter";
	  protected internal const string CANDIDATE_STARTER_USERS_EXTENSION = "candidateStarterUsers";
	  protected internal const string CANDIDATE_STARTER_GROUPS_EXTENSION = "candidateStarterGroups";

	  protected internal static readonly string ATTRIBUTEVALUE_T_FORMAL_EXPRESSION = BpmnParser.BPMN20_NS + ":tFormalExpression";

	  public const string PROPERTYNAME_IS_MULTI_INSTANCE = "isMultiInstance";

	  public static readonly Namespace CAMUNDA_BPMN_EXTENSIONS_NS = new Namespace(BpmnParser.CAMUNDA_BPMN_EXTENSIONS_NS, BpmnParser.ACTIVITI_BPMN_EXTENSIONS_NS);
	  public static readonly Namespace XSI_NS = new Namespace(BpmnParser.XSI_NS);
	  public static readonly Namespace BPMN_DI_NS = new Namespace(BpmnParser.BPMN_DI_NS);
	  public static readonly Namespace OMG_DI_NS = new Namespace(BpmnParser.OMG_DI_NS);
	  public static readonly Namespace BPMN_DC_NS = new Namespace(BpmnParser.BPMN_DC_NS);
	  public const string ALL = "all";

	  /// <summary>
	  /// The deployment to which the parsed process definitions will be added. </summary>
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal DeploymentEntity deployment_Renamed;

	  /// <summary>
	  /// The end result of the parsing: a list of process definition. </summary>
	  protected internal IList<ProcessDefinitionEntity> processDefinitions = new List<ProcessDefinitionEntity>();

	  /// <summary>
	  /// Mapping of found errors in BPMN 2.0 file </summary>
	  protected internal new IDictionary<string, Error> errors = new Dictionary<string, Error>();

	  /// <summary>
	  /// Mapping of found escalation elements </summary>
	  protected internal IDictionary<string, Escalation> escalations = new Dictionary<string, Escalation>();

	  /// <summary>
	  /// Mapping from a process definition key to his containing list of job
	  /// declarations
	  /// 
	  /// </summary>
//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: protected Map<String, List<JobDeclaration<?, ?>>> jobDeclarations = new HashMap<String, List<JobDeclaration<?, ?>>>();
	  protected internal IDictionary<string, IList<JobDeclaration<object, ?>>> jobDeclarations = new Dictionary<string, IList<JobDeclaration<object, ?>>>();

	  /// <summary>
	  /// A map for storing sequence flow based on their id during parsing. </summary>
	  protected internal IDictionary<string, TransitionImpl> sequenceFlows;

	  /// <summary>
	  /// A list of all element IDs. This allows us to parse only what we actually
	  /// support but still validate the references among elements we do not support.
	  /// </summary>
	  protected internal IList<string> elementIds = new List<string>();

	  /// <summary>
	  /// A map for storing the process references of participants </summary>
	  protected internal IDictionary<string, string> participantProcesses = new Dictionary<string, string>();

	  /// <summary>
	  /// Mapping containing values stored during the first phase of parsing since
	  /// other elements can reference these messages.
	  /// 
	  /// All the map's elements are defined outside the process definition(s), which
	  /// means that this map doesn't need to be re-initialized for each new process
	  /// definition.
	  /// </summary>
	  protected internal IDictionary<string, MessageDefinition> messages = new Dictionary<string, MessageDefinition>();
	  protected internal IDictionary<string, SignalDefinition> signals = new Dictionary<string, SignalDefinition>();

	  // Members
	  protected internal ExpressionManager expressionManager;
	  protected internal IList<BpmnParseListener> parseListeners;
	  protected internal IDictionary<string, XMLImporter> importers = new Dictionary<string, XMLImporter>();
	  protected internal IDictionary<string, string> prefixs = new Dictionary<string, string>();
	  protected internal string targetNamespace;

	  private IDictionary<string, string> eventLinkTargets = new Dictionary<string, string>();
	  private IDictionary<string, string> eventLinkSources = new Dictionary<string, string>();

	  /// <summary>
	  /// Constructor to be called by the <seealso cref="BpmnParser"/>.
	  /// </summary>
	  public BpmnParse(BpmnParser parser) : base(parser)
	  {
		this.expressionManager = parser.ExpressionManager;
		this.parseListeners = parser.ParseListeners;
		SchemaResource = ReflectUtil.getResourceUrlAsString(BpmnParser.BPMN_20_SCHEMA_LOCATION);
		EnableXxeProcessing = Context.ProcessEngineConfiguration.EnableXxeProcessing;
	  }

	  public virtual BpmnParse deployment(DeploymentEntity deployment)
	  {
		this.deployment_Renamed = deployment;
		return this;
	  }

	  public override BpmnParse execute()
	  {
		base.execute(); // schema validation

		try
		{
		  parseRootElement();
		}
		catch (BpmnParseException e)
		{
		  addError(e);

		}
		catch (Exception e)
		{
		  LOG.parsingFailure(e);

		  // ALL unexpected exceptions should bubble up since they are not handled
		  // accordingly by underlying parse-methods and the process can't be
		  // deployed
		  throw LOG.parsingProcessException(e);

		}
		finally
		{
		  if (hasWarnings())
		  {
			logWarnings();
		  }
		  if (hasErrors())
		  {
			throwExceptionForErrors();
		  }
		}

		return this;
	  }

	  /// <summary>
	  /// Parses the 'definitions' root element
	  /// </summary>
	  protected internal virtual void parseRootElement()
	  {
		collectElementIds();
		parseDefinitionsAttributes();
		parseImports();
		parseMessages();
		parseSignals();
		parseErrors();
		parseEscalations();
		parseProcessDefinitions();
		parseCollaboration();

		// Diagram interchange parsing must be after parseProcessDefinitions,
		// since it depends and sets values on existing process definition objects
		parseDiagramInterchangeElements();

		foreach (BpmnParseListener parseListener in parseListeners)
		{
		  parseListener.parseRootElement(rootElement, ProcessDefinitions);
		}
	  }

	  protected internal virtual void collectElementIds()
	  {
		rootElement.collectIds(elementIds);
	  }

	  protected internal virtual void parseDefinitionsAttributes()
	  {
		this.targetNamespace = rootElement.attribute("targetNamespace");

		foreach (string attribute in rootElement.attributes())
		{
		  if (attribute.StartsWith("xmlns:", StringComparison.Ordinal))
		  {
			string prefixValue = rootElement.attribute(attribute);
			string prefixName = attribute.Substring(6);
			this.prefixs[prefixName] = prefixValue;
		  }
		}
	  }

	  protected internal virtual string resolveName(string name)
	  {
		if (string.ReferenceEquals(name, null))
		{
		  return null;
		}
		int indexOfP = name.IndexOf(':');
		if (indexOfP != -1)
		{
		  string prefix = name.Substring(0, indexOfP);
		  string resolvedPrefix = this.prefixs[prefix];
		  return resolvedPrefix + ":" + name.Substring(indexOfP + 1);
		}
		else
		{
		  return this.targetNamespace + ":" + name;
		}
	  }

	  /// <summary>
	  /// Parses the rootElement importing structures
	  /// </summary>
	  protected internal virtual void parseImports()
	  {
		IList<Element> imports = rootElement.elements("import");
		foreach (Element theImport in imports)
		{
		  string importType = theImport.attribute("importType");
		  XMLImporter importer = this.getImporter(importType, theImport);
		  if (importer == null)
		  {
			addError("Could not import item of type " + importType, theImport);
		  }
		  else
		  {
			importer.importFrom(theImport, this);
		  }
		}
	  }

	  protected internal virtual XMLImporter getImporter(string importType, Element theImport)
	  {
		if (this.importers.ContainsKey(importType))
		{
		  return this.importers[importType];
		}
		else
		{
		  if (importType.Equals("http://schemas.xmlsoap.org/wsdl/"))
		  {
			Type wsdlImporterClass;
			try
			{
			  wsdlImporterClass = Type.GetType("org.camunda.bpm.engine.impl.webservice.CxfWSDLImporter", true, Thread.CurrentThread.ContextClassLoader);
			  XMLImporter newInstance = (XMLImporter) System.Activator.CreateInstance(wsdlImporterClass);
			  this.importers[importType] = newInstance;
			  return newInstance;
			}
			catch (Exception)
			{
			  addError("Could not find importer for type " + importType, theImport);
			}
		  }
		  return null;
		}
	  }

	  /// <summary>
	  /// Parses the messages of the given definitions file. Messages are not
	  /// contained within a process element, but they can be referenced from inner
	  /// process elements.
	  /// </summary>
	  public virtual void parseMessages()
	  {
		foreach (Element messageElement in rootElement.elements("message"))
		{
		  string id = messageElement.attribute("id");
		  string messageName = messageElement.attribute("name");

		  Expression messageExpression = null;
		  if (!string.ReferenceEquals(messageName, null))
		  {
			messageExpression = expressionManager.createExpression(messageName);
		  }

		  MessageDefinition messageDefinition = new MessageDefinition(this.targetNamespace + ":" + id, messageExpression);
		  this.messages[messageDefinition.Id] = messageDefinition;
		}
	  }

	  /// <summary>
	  /// Parses the signals of the given definitions file. Signals are not contained
	  /// within a process element, but they can be referenced from inner process
	  /// elements.
	  /// </summary>
	  protected internal virtual void parseSignals()
	  {
		foreach (Element signalElement in rootElement.elements("signal"))
		{
		  string id = signalElement.attribute("id");
		  string signalName = signalElement.attribute("name");

		  foreach (SignalDefinition signalDefinition in signals.Values)
		  {
			if (signalDefinition.Name.Equals(signalName))
			{
			  addError("duplicate signal name '" + signalName + "'.", signalElement);
			}
		  }

		  if (string.ReferenceEquals(id, null))
		  {
			addError("signal must have an id", signalElement);
		  }
		  else if (string.ReferenceEquals(signalName, null))
		  {
			addError("signal with id '" + id + "' has no name", signalElement);
		  }
		  else
		  {
			Expression signalExpression = expressionManager.createExpression(signalName);
			SignalDefinition signal = new SignalDefinition();
			signal.Id = this.targetNamespace + ":" + id;
			signal.Expression = signalExpression;

			this.signals[signal.Id] = signal;
		  }
		}
	  }

	  public virtual void parseErrors()
	  {
		foreach (Element errorElement in rootElement.elements("error"))
		{
		  Error error = new Error();

		  string id = errorElement.attribute("id");
		  if (string.ReferenceEquals(id, null))
		  {
			addError("'id' is mandatory on error definition", errorElement);
		  }
		  error.Id = id;

		  string errorCode = errorElement.attribute("errorCode");
		  if (!string.ReferenceEquals(errorCode, null))
		  {
			error.ErrorCode = errorCode;
		  }

		  errors[id] = error;
		}
	  }

	  protected internal virtual void parseEscalations()
	  {
		foreach (Element element in rootElement.elements("escalation"))
		{

		  string id = element.attribute("id");
		  if (string.ReferenceEquals(id, null))
		  {
			addError("escalation must have an id", element);
		  }
		  else
		  {

			Escalation escalation = createEscalation(id, element);
			escalations[id] = escalation;
		  }
		}
	  }

	  protected internal virtual Escalation createEscalation(string id, Element element)
	  {

		Escalation escalation = new Escalation(id);

		string name = element.attribute("name");
		if (!string.ReferenceEquals(name, null))
		{
		  escalation.Name = name;
		}

		string escalationCode = element.attribute("escalationCode");
		if (!string.ReferenceEquals(escalationCode, null) && escalationCode.Length > 0)
		{
		  escalation.EscalationCode = escalationCode;
		}
		return escalation;
	  }

	  /// <summary>
	  /// Parses all the process definitions defined within the 'definitions' root
	  /// element.
	  /// </summary>
	  public virtual void parseProcessDefinitions()
	  {
		foreach (Element processElement in rootElement.elements("process"))
		{
		  bool isExecutable = !deployment_Renamed.New;
		  string isExecutableStr = processElement.attribute("isExecutable");
		  if (!string.ReferenceEquals(isExecutableStr, null))
		  {
			isExecutable = bool.Parse(isExecutableStr);
			if (!isExecutable)
			{
			  LOG.ignoringNonExecutableProcess(processElement.attribute("id"));
			}
		  }
		  else
		  {
			LOG.missingIsExecutableAttribute(processElement.attribute("id"));
		  }

		  // Only process executable processes
		  if (isExecutable)
		  {
			processDefinitions.Add(parseProcess(processElement));
		  }
		}
	  }

	  /// <summary>
	  /// Parses the collaboration definition defined within the 'definitions' root
	  /// element and get all participants to lookup their process references during
	  /// DI parsing.
	  /// </summary>
	  public virtual void parseCollaboration()
	  {
		Element collaboration = rootElement.element("collaboration");
		if (collaboration != null)
		{
		  foreach (Element participant in collaboration.elements("participant"))
		  {
			string processRef = participant.attribute("processRef");
			if (!string.ReferenceEquals(processRef, null))
			{
			  ProcessDefinitionImpl procDef = getProcessDefinition(processRef);
			  if (procDef != null)
			  {
				// Set participant process on the procDef, so it can get rendered
				// later on if needed
				ParticipantProcess participantProcess = new ParticipantProcess();
				participantProcess.Id = participant.attribute("id");
				participantProcess.Name = participant.attribute("name");
				procDef.ParticipantProcess = participantProcess;

				participantProcesses[participantProcess.Id] = processRef;
			  }
			}
		  }
		}
	  }

	  /// <summary>
	  /// Parses one process (ie anything inside a &lt;process&gt; element).
	  /// </summary>
	  /// <param name="processElement">
	  ///          The 'process' element. </param>
	  /// <returns> The parsed version of the XML: a <seealso cref="ProcessDefinitionImpl"/>
	  ///         object. </returns>
	  public virtual ProcessDefinitionEntity parseProcess(Element processElement)
	  {
		// reset all mappings that are related to one process definition
		sequenceFlows = new Dictionary<string, TransitionImpl>();

		ProcessDefinitionEntity processDefinition = new ProcessDefinitionEntity();

		/*
		 * Mapping object model - bpmn xml: processDefinition.id -> generated by
		 * processDefinition.key -> bpmn id (required) processDefinition.name ->
		 * bpmn name (optional)
		 */
		processDefinition.Key = processElement.attribute("id");
		processDefinition.Name = processElement.attribute("name");
		processDefinition.Category = rootElement.attribute("targetNamespace");
		processDefinition.setProperty(PROPERTYNAME_DOCUMENTATION, parseDocumentation(processElement));
		processDefinition.TaskDefinitions = new Dictionary<string, TaskDefinition>();
		processDefinition.DeploymentId = deployment_Renamed.Id;
		processDefinition.setProperty(PROPERTYNAME_JOB_PRIORITY, parsePriority(processElement, PROPERTYNAME_JOB_PRIORITY));
		processDefinition.setProperty(PROPERTYNAME_TASK_PRIORITY, parsePriority(processElement, PROPERTYNAME_TASK_PRIORITY));
		processDefinition.VersionTag = processElement.attributeNS(CAMUNDA_BPMN_EXTENSIONS_NS, "versionTag");

		try
		{
		  string historyTimeToLive = processElement.attributeNS(CAMUNDA_BPMN_EXTENSIONS_NS, "historyTimeToLive", Context.ProcessEngineConfiguration.HistoryTimeToLive);
		  processDefinition.HistoryTimeToLive = ParseUtil.parseHistoryTimeToLive(historyTimeToLive);
		}
		catch (Exception e)
		{
		  addError(new BpmnParseException(e.Message, processElement, e));
		}

		bool isStartableInTasklist = isStartable(processElement);
		processDefinition.StartableInTasklist = isStartableInTasklist;

		LOG.parsingElement("process", processDefinition.Key);

		parseScope(processElement, processDefinition);

		// Parse any laneSets defined for this process
		parseLaneSets(processElement, processDefinition);

		foreach (BpmnParseListener parseListener in parseListeners)
		{
		  parseListener.parseProcess(processElement, processDefinition);
		}

		// now we have parsed anything we can validate some stuff
		validateActivities(processDefinition.Activities);

		//unregister delegates
		foreach (ActivityImpl activity in processDefinition.Activities)
		{
		  activity.DelegateAsyncAfterUpdate = null;
		  activity.DelegateAsyncBeforeUpdate = null;
		}
		return processDefinition;
	  }

	  protected internal virtual void parseLaneSets(Element parentElement, ProcessDefinitionEntity processDefinition)
	  {
		IList<Element> laneSets = parentElement.elements("laneSet");

		if (laneSets != null && laneSets.Count > 0)
		{
		  foreach (Element laneSetElement in laneSets)
		  {
			LaneSet newLaneSet = new LaneSet();

			newLaneSet.Id = laneSetElement.attribute("id");
			newLaneSet.Name = laneSetElement.attribute("name");
			parseLanes(laneSetElement, newLaneSet);

			// Finally, add the set
			processDefinition.addLaneSet(newLaneSet);
		  }
		}
	  }

	  protected internal virtual void parseLanes(Element laneSetElement, LaneSet laneSet)
	  {
		IList<Element> lanes = laneSetElement.elements("lane");
		if (lanes != null && lanes.Count > 0)
		{
		  foreach (Element laneElement in lanes)
		  {
			// Parse basic attributes
			Lane lane = new Lane();
			lane.Id = laneElement.attribute("id");
			lane.Name = laneElement.attribute("name");

			// Parse ID's of flow-nodes that live inside this lane
			IList<Element> flowNodeElements = laneElement.elements("flowNodeRef");
			if (flowNodeElements != null && flowNodeElements.Count > 0)
			{
			  foreach (Element flowNodeElement in flowNodeElements)
			  {
				lane.FlowNodeIds.Add(flowNodeElement.Text);
			  }
			}

			laneSet.addLane(lane);
		  }
		}
	  }

	  /// <summary>
	  /// Parses a scope: a process, subprocess, etc.
	  /// 
	  /// Note that a process definition is a scope on itself.
	  /// </summary>
	  /// <param name="scopeElement">
	  ///          The XML element defining the scope </param>
	  /// <param name="parentScope">
	  ///          The scope that contains the nested scope. </param>
	  public virtual void parseScope(Element scopeElement, ScopeImpl parentScope)
	  {

		// Not yet supported on process level (PVM additions needed):
		// parseProperties(processElement);

		// filter activities that must be parsed separately
		IList<Element> activityElements = new List<Element>(scopeElement.elements());
		IDictionary<string, Element> intermediateCatchEvents = filterIntermediateCatchEvents(activityElements);
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the java.util.Collection 'removeAll' method:
		activityElements.removeAll(intermediateCatchEvents.Values);
		IDictionary<string, Element> compensationHandlers = filterCompensationHandlers(activityElements);
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the java.util.Collection 'removeAll' method:
		activityElements.removeAll(compensationHandlers.Values);

		parseStartEvents(scopeElement, parentScope);
		parseActivities(activityElements, scopeElement, parentScope);
		parseIntermediateCatchEvents(scopeElement, parentScope, intermediateCatchEvents);
		parseEndEvents(scopeElement, parentScope);
		parseBoundaryEvents(scopeElement, parentScope);
		parseSequenceFlow(scopeElement, parentScope, compensationHandlers);
		parseExecutionListenersOnScope(scopeElement, parentScope);
		parseAssociations(scopeElement, parentScope, compensationHandlers);
		parseCompensationHandlers(parentScope, compensationHandlers);

		foreach (ScopeImpl.BacklogErrorCallback callback in parentScope.BacklogErrorCallbacks)
		{
		  callback.callback();
		}

		if (parentScope is ProcessDefinition)
		{
		  parseProcessDefinitionCustomExtensions(scopeElement, (ProcessDefinition) parentScope);
		}
	  }

	  protected internal virtual Dictionary<string, Element> filterIntermediateCatchEvents(IList<Element> activityElements)
	  {
		Dictionary<string, Element> intermediateCatchEvents = new Dictionary<string, Element>();
		foreach (Element activityElement in activityElements)
		{
		  if (activityElement.TagName.Equals(ActivityTypes.INTERMEDIATE_EVENT_CATCH))
		  {
			intermediateCatchEvents[activityElement.attribute("id")] = activityElement;
		  }
		}
		return intermediateCatchEvents;
	  }

	  protected internal virtual Dictionary<string, Element> filterCompensationHandlers(IList<Element> activityElements)
	  {
		Dictionary<string, Element> compensationHandlers = new Dictionary<string, Element>();
		foreach (Element activityElement in activityElements)
		{
		  if (isCompensationHandler(activityElement))
		  {
			compensationHandlers[activityElement.attribute("id")] = activityElement;
		  }
		}
		return compensationHandlers;
	  }

	  protected internal virtual void parseIntermediateCatchEvents(Element scopeElement, ScopeImpl parentScope, IDictionary<string, Element> intermediateCatchEventElements)
	  {
		foreach (Element intermediateCatchEventElement in intermediateCatchEventElements.Values)
		{

		  if (parentScope.findActivity(intermediateCatchEventElement.attribute("id")) == null)
		  {
			// check whether activity is already parsed
			ActivityImpl activity = parseIntermediateCatchEvent(intermediateCatchEventElement, parentScope, null);

			if (activity != null)
			{
			  parseActivityInputOutput(intermediateCatchEventElement, activity);
			}
		  }
		}
		intermediateCatchEventElements.Clear();
	  }

	  protected internal virtual void parseProcessDefinitionCustomExtensions(Element scopeElement, ProcessDefinition definition)
	  {
		parseStartAuthorization(scopeElement, definition);
	  }

	  protected internal virtual void parseStartAuthorization(Element scopeElement, ProcessDefinition definition)
	  {
		ProcessDefinitionEntity processDefinition = (ProcessDefinitionEntity) definition;

		// parse activiti:potentialStarters
		Element extentionsElement = scopeElement.element("extensionElements");
		if (extentionsElement != null)
		{
		  IList<Element> potentialStarterElements = extentionsElement.elementsNS(CAMUNDA_BPMN_EXTENSIONS_NS, POTENTIAL_STARTER);

		  foreach (Element potentialStarterElement in potentialStarterElements)
		  {
			parsePotentialStarterResourceAssignment(potentialStarterElement, processDefinition);
		  }
		}

		// parse activiti:candidateStarterUsers
		string candidateUsersString = scopeElement.attributeNS(CAMUNDA_BPMN_EXTENSIONS_NS, CANDIDATE_STARTER_USERS_EXTENSION);
		if (!string.ReferenceEquals(candidateUsersString, null))
		{
		  IList<string> candidateUsers = parseCommaSeparatedList(candidateUsersString);
		  foreach (string candidateUser in candidateUsers)
		  {
			processDefinition.addCandidateStarterUserIdExpression(expressionManager.createExpression(candidateUser.Trim()));
		  }
		}

		// Candidate activiti:candidateStarterGroups
		string candidateGroupsString = scopeElement.attributeNS(CAMUNDA_BPMN_EXTENSIONS_NS, CANDIDATE_STARTER_GROUPS_EXTENSION);
		if (!string.ReferenceEquals(candidateGroupsString, null))
		{
		  IList<string> candidateGroups = parseCommaSeparatedList(candidateGroupsString);
		  foreach (string candidateGroup in candidateGroups)
		  {
			processDefinition.addCandidateStarterGroupIdExpression(expressionManager.createExpression(candidateGroup.Trim()));
		  }
		}

	  }

	  protected internal virtual void parsePotentialStarterResourceAssignment(Element performerElement, ProcessDefinitionEntity processDefinition)
	  {
		Element raeElement = performerElement.element(RESOURCE_ASSIGNMENT_EXPR);
		if (raeElement != null)
		{
		  Element feElement = raeElement.element(FORMAL_EXPRESSION);
		  if (feElement != null)
		  {
			IList<string> assignmentExpressions = parseCommaSeparatedList(feElement.Text);
			foreach (string assignmentExpression in assignmentExpressions)
			{
			  assignmentExpression = assignmentExpression.Trim();
			  if (assignmentExpression.StartsWith(USER_PREFIX, StringComparison.Ordinal))
			  {
				string userAssignementId = getAssignmentId(assignmentExpression, USER_PREFIX);
				processDefinition.addCandidateStarterUserIdExpression(expressionManager.createExpression(userAssignementId));
			  }
			  else if (assignmentExpression.StartsWith(GROUP_PREFIX, StringComparison.Ordinal))
			  {
				string groupAssignementId = getAssignmentId(assignmentExpression, GROUP_PREFIX);
				processDefinition.addCandidateStarterGroupIdExpression(expressionManager.createExpression(groupAssignementId));
			  }
			  else
			  { // default: given string is a goupId, as-is.
				processDefinition.addCandidateStarterGroupIdExpression(expressionManager.createExpression(assignmentExpression));
			  }
			}
		  }
		}
	  }

	  protected internal virtual void parseAssociations(Element scopeElement, ScopeImpl parentScope, IDictionary<string, Element> compensationHandlers)
	  {
		foreach (Element associationElement in scopeElement.elements("association"))
		{
		  string sourceRef = associationElement.attribute("sourceRef");
		  if (string.ReferenceEquals(sourceRef, null))
		  {
			addError("association element missing attribute 'sourceRef'", associationElement);
		  }
		  string targetRef = associationElement.attribute("targetRef");
		  if (string.ReferenceEquals(targetRef, null))
		  {
			addError("association element missing attribute 'targetRef'", associationElement);
		  }
		  ActivityImpl sourceActivity = parentScope.findActivity(sourceRef);
		  ActivityImpl targetActivity = parentScope.findActivity(targetRef);

		  // an association may reference elements that are not parsed as activities
		  // (like for instance text annotations so do not throw an exception if sourceActivity or targetActivity are null)
		  // However, we make sure they reference 'something':
		  if (sourceActivity == null && !elementIds.Contains(sourceRef))
		  {
			addError("Invalid reference sourceRef '" + sourceRef + "' of association element ", associationElement);
		  }
		  else if (targetActivity == null && !elementIds.Contains(targetRef))
		  {
			addError("Invalid reference targetRef '" + targetRef + "' of association element ", associationElement);
		  }
		  else
		  {

			if (sourceActivity != null && ActivityTypes.BOUNDARY_COMPENSATION.Equals(sourceActivity.getProperty(BpmnProperties.TYPE.Name)))
			{

			  if (targetActivity == null && compensationHandlers.ContainsKey(targetRef))
			  {
				targetActivity = parseCompensationHandlerForCompensationBoundaryEvent(parentScope, sourceActivity, targetRef, compensationHandlers);

				compensationHandlers.Remove(targetActivity.Id);
			  }

			  if (targetActivity != null)
			  {
				parseAssociationOfCompensationBoundaryEvent(associationElement, sourceActivity, targetActivity);
			  }
			}
		  }
		}
	  }

	  protected internal virtual ActivityImpl parseCompensationHandlerForCompensationBoundaryEvent(ScopeImpl parentScope, ActivityImpl sourceActivity, string targetRef, IDictionary<string, Element> compensationHandlers)
	  {

		Element compensationHandler = compensationHandlers[targetRef];

		ActivityImpl eventScope = (ActivityImpl) sourceActivity.EventScope;
		ActivityImpl compensationHandlerActivity = null;
		if (eventScope.MultiInstance)
		{
		  ScopeImpl miBody = eventScope.FlowScope;
		  compensationHandlerActivity = parseActivity(compensationHandler, null, miBody);
		}
		else
		{
		  compensationHandlerActivity = parseActivity(compensationHandler, null, parentScope);
		}

		compensationHandlerActivity.Properties.set(BpmnProperties.COMPENSATION_BOUNDARY_EVENT, sourceActivity);
		return compensationHandlerActivity;
	  }

	  protected internal virtual void parseAssociationOfCompensationBoundaryEvent(Element associationElement, ActivityImpl sourceActivity, ActivityImpl targetActivity)
	  {
		if (!targetActivity.CompensationHandler)
		{
		  addError("compensation boundary catch must be connected to element with isForCompensation=true", associationElement);

		}
		else
		{
		  ActivityImpl compensatedActivity = (ActivityImpl) sourceActivity.EventScope;

		  ActivityImpl compensationHandler = compensatedActivity.findCompensationHandler();
		  if (compensationHandler != null && compensationHandler.SubProcessScope)
		  {
			addError("compensation boundary event and event subprocess with compensation start event are not supported on the same scope", associationElement);
		  }
		  else
		  {

			compensatedActivity.setProperty(PROPERTYNAME_COMPENSATION_HANDLER_ID, targetActivity.Id);
		  }
		}
	  }

	  protected internal virtual void parseCompensationHandlers(ScopeImpl parentScope, IDictionary<string, Element> compensationHandlers)
	  {
		// compensation handlers attached to compensation boundary events should be already parsed
		foreach (Element compensationHandler in new HashSet<Element>(compensationHandlers.Values))
		{
		  parseActivity(compensationHandler, null, parentScope);
		}
		compensationHandlers.Clear();
	  }

	  /// <summary>
	  /// Parses the start events of a certain level in the process (process,
	  /// subprocess or another scope).
	  /// </summary>
	  /// <param name="parentElement">
	  ///          The 'parent' element that contains the start events (process,
	  ///          subprocess). </param>
	  /// <param name="scope">
	  ///          The <seealso cref="ScopeImpl"/> to which the start events must be added. </param>
	  public virtual void parseStartEvents(Element parentElement, ScopeImpl scope)
	  {
		IList<Element> startEventElements = parentElement.elements("startEvent");
		IList<ActivityImpl> startEventActivities = new List<ActivityImpl>();
		foreach (Element startEventElement in startEventElements)
		{

		  ActivityImpl startEventActivity = createActivityOnScope(startEventElement, scope);
		  parseAsynchronousContinuationForActivity(startEventElement, startEventActivity);

		  if (scope is ProcessDefinitionEntity)
		  {
			parseProcessDefinitionStartEvent(startEventActivity, startEventElement, parentElement, scope);
			startEventActivities.Add(startEventActivity);
		  }
		  else
		  {
			parseScopeStartEvent(startEventActivity, startEventElement, parentElement, (ActivityImpl) scope);
		  }

		  ensureNoIoMappingDefined(startEventElement);

		  parseExecutionListenersOnScope(startEventElement, startEventActivity);

		  foreach (BpmnParseListener parseListener in parseListeners)
		  {
			parseListener.parseStartEvent(startEventElement, scope, startEventActivity);
		  }

		}

		if (scope is ProcessDefinitionEntity)
		{
		  selectInitial(startEventActivities, (ProcessDefinitionEntity) scope, parentElement);
		  parseStartFormHandlers(startEventElements, (ProcessDefinitionEntity) scope);
		}
	  }

	  protected internal virtual void selectInitial(IList<ActivityImpl> startEventActivities, ProcessDefinitionEntity processDefinition, Element parentElement)
	  {
		ActivityImpl initial = null;
		// validate that there is s single none start event / timer start event:
		IList<string> exclusiveStartEventTypes = new IList<string> {"startEvent", "startTimerEvent"};

		foreach (ActivityImpl activityImpl in startEventActivities)
		{
		  if (exclusiveStartEventTypes.Contains(activityImpl.getProperty(BpmnProperties.TYPE.Name)))
		  {
			if (initial == null)
			{
			  initial = activityImpl;
			}
			else
			{
			  addError("multiple none start events or timer start events not supported on process definition", parentElement);
			}
		  }
		}
		// if there is a single start event, select it as initial, regardless of it's type:
		if (initial == null && startEventActivities.Count == 1)
		{
		  initial = startEventActivities[0];
		}
		processDefinition.Initial = initial;
	  }

	  protected internal virtual void parseProcessDefinitionStartEvent(ActivityImpl startEventActivity, Element startEventElement, Element parentElement, ScopeImpl scope)
	  {
		ProcessDefinitionEntity processDefinition = (ProcessDefinitionEntity) scope;

		string initiatorVariableName = startEventElement.attributeNS(CAMUNDA_BPMN_EXTENSIONS_NS, "initiator");
		if (!string.ReferenceEquals(initiatorVariableName, null))
		{
		  processDefinition.setProperty(PROPERTYNAME_INITIATOR_VARIABLE_NAME, initiatorVariableName);
		}

		// all start events share the same behavior:
		startEventActivity.ActivityBehavior = new NoneStartEventActivityBehavior();

		Element timerEventDefinition = startEventElement.element(TIMER_EVENT_DEFINITION);
		Element messageEventDefinition = startEventElement.element(MESSAGE_EVENT_DEFINITION);
		Element signalEventDefinition = startEventElement.element(SIGNAL_EVENT_DEFINITION);
		Element conditionEventDefinition = startEventElement.element(CONDITIONAL_EVENT_DEFINITION);
		if (timerEventDefinition != null)
		{
		  parseTimerStartEventDefinition(timerEventDefinition, startEventActivity, processDefinition);
		}
		else if (messageEventDefinition != null)
		{
		  startEventActivity.Properties.set(BpmnProperties.TYPE, ActivityTypes.START_EVENT_MESSAGE);

		  EventSubscriptionDeclaration messageStartEventSubscriptionDeclaration = parseMessageEventDefinition(messageEventDefinition);
		  messageStartEventSubscriptionDeclaration.ActivityId = startEventActivity.Id;
		  messageStartEventSubscriptionDeclaration.StartEvent = true;

		  ensureNoExpressionInMessageStartEvent(messageEventDefinition, messageStartEventSubscriptionDeclaration);
		  addEventSubscriptionDeclaration(messageStartEventSubscriptionDeclaration, processDefinition, startEventElement);
		}
		else if (signalEventDefinition != null)
		{
		  startEventActivity.Properties.set(BpmnProperties.TYPE, ActivityTypes.START_EVENT_SIGNAL);
		  startEventActivity.EventScope = scope;

		  parseSignalCatchEventDefinition(signalEventDefinition, startEventActivity, true);
		}
		else if (conditionEventDefinition != null)
		{
		  startEventActivity.Properties.set(BpmnProperties.TYPE, ActivityTypes.START_EVENT_CONDITIONAL);

		  ConditionalEventDefinition conditionalEventDefinition = parseConditionalEventDefinition(conditionEventDefinition, startEventActivity);
		  conditionalEventDefinition.StartEvent = true;
		  conditionalEventDefinition.ActivityId = startEventActivity.Id;
		  startEventActivity.Properties.set(BpmnProperties.CONDITIONAL_EVENT_DEFINITION, conditionalEventDefinition);

		  addEventSubscriptionDeclaration(conditionalEventDefinition, processDefinition, startEventElement);
		}
	  }

	  protected internal virtual void parseStartFormHandlers(IList<Element> startEventElements, ProcessDefinitionEntity processDefinition)
	  {
		if (processDefinition.Initial != null)
		{
		  foreach (Element startEventElement in startEventElements)
		  {

			if (startEventElement.attribute("id").Equals(processDefinition.Initial.Id))
			{

			  StartFormHandler startFormHandler;
			  string startFormHandlerClassName = startEventElement.attributeNS(CAMUNDA_BPMN_EXTENSIONS_NS, "formHandlerClass");
			  if (!string.ReferenceEquals(startFormHandlerClassName, null))
			  {
				startFormHandler = (StartFormHandler) ReflectUtil.instantiate(startFormHandlerClassName);
			  }
			  else
			  {
				startFormHandler = new DefaultStartFormHandler();
			  }
			  startFormHandler.parseConfiguration(startEventElement, deployment_Renamed, processDefinition, this);

			  processDefinition.StartFormHandler = new DelegateStartFormHandler(startFormHandler, deployment_Renamed);
			}

		  }
		}
	  }

	  protected internal virtual void parseScopeStartEvent(ActivityImpl startEventActivity, Element startEventElement, Element parentElement, ActivityImpl scopeActivity)
	  {

		Properties scopeProperties = scopeActivity.Properties;

		// set this as the scope's initial
		if (!scopeProperties.contains(BpmnProperties.INITIAL_ACTIVITY))
		{
		  scopeProperties.set(BpmnProperties.INITIAL_ACTIVITY, startEventActivity);
		}
		else
		{
		  addError("multiple start events not supported for subprocess", startEventElement);
		}

		Element errorEventDefinition = startEventElement.element(ERROR_EVENT_DEFINITION);
		Element messageEventDefinition = startEventElement.element(MESSAGE_EVENT_DEFINITION);
		Element signalEventDefinition = startEventElement.element(SIGNAL_EVENT_DEFINITION);
		Element timerEventDefinition = startEventElement.element(TIMER_EVENT_DEFINITION);
		Element compensateEventDefinition = startEventElement.element(COMPENSATE_EVENT_DEFINITION);
		Element escalationEventDefinitionElement = startEventElement.element(ESCALATION_EVENT_DEFINITION);
		Element conditionalEventDefinitionElement = startEventElement.element(CONDITIONAL_EVENT_DEFINITION);

		if (scopeActivity.TriggeredByEvent)
		{
		  // event subprocess
		  EventSubProcessStartEventActivityBehavior behavior = new EventSubProcessStartEventActivityBehavior();

		  // parse isInterrupting
		  string isInterruptingAttr = startEventElement.attribute(INTERRUPTING);
		  bool isInterrupting = isInterruptingAttr.Equals(TRUE, StringComparison.OrdinalIgnoreCase);

		  if (isInterrupting)
		  {
			scopeActivity.ActivityStartBehavior = ActivityStartBehavior.INTERRUPT_EVENT_SCOPE;
		  }
		  else
		  {
			scopeActivity.ActivityStartBehavior = ActivityStartBehavior.CONCURRENT_IN_FLOW_SCOPE;
		  }

		  // the event scope of the start event is the flow scope of the event subprocess
		  startEventActivity.EventScope = scopeActivity.FlowScope;

		  if (errorEventDefinition != null)
		  {
			if (!isInterrupting)
			{
			  addError("error start event of event subprocess must be interrupting", startEventElement);
			}
			parseErrorStartEventDefinition(errorEventDefinition, startEventActivity);

		  }
		  else if (messageEventDefinition != null)
		  {
			startEventActivity.Properties.set(BpmnProperties.TYPE, ActivityTypes.START_EVENT_MESSAGE);

			EventSubscriptionDeclaration messageStartEventSubscriptionDeclaration = parseMessageEventDefinition(messageEventDefinition);
			parseEventDefinitionForSubprocess(messageStartEventSubscriptionDeclaration, startEventActivity, messageEventDefinition);

		  }
		  else if (signalEventDefinition != null)
		  {
			startEventActivity.Properties.set(BpmnProperties.TYPE, ActivityTypes.START_EVENT_SIGNAL);

			EventSubscriptionDeclaration eventSubscriptionDeclaration = parseSignalEventDefinition(signalEventDefinition, false);
			parseEventDefinitionForSubprocess(eventSubscriptionDeclaration, startEventActivity, signalEventDefinition);

		  }
		  else if (timerEventDefinition != null)
		  {
			parseTimerStartEventDefinitionForEventSubprocess(timerEventDefinition, startEventActivity, isInterrupting);

		  }
		  else if (compensateEventDefinition != null)
		  {
			parseCompensationEventSubprocess(startEventActivity, startEventElement, scopeActivity, compensateEventDefinition);

		  }
		  else if (escalationEventDefinitionElement != null)
		  {
			startEventActivity.Properties.set(BpmnProperties.TYPE, ActivityTypes.START_EVENT_ESCALATION);

			EscalationEventDefinition escalationEventDefinition = createEscalationEventDefinitionForEscalationHandler(escalationEventDefinitionElement, scopeActivity, isInterrupting);
			addEscalationEventDefinition(startEventActivity.EventScope, escalationEventDefinition, escalationEventDefinitionElement);
		  }
		  else if (conditionalEventDefinitionElement != null)
		  {

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final ConditionalEventDefinition conditionalEventDef = parseConditionalStartEventForEventSubprocess(conditionalEventDefinitionElement, startEventActivity, isInterrupting);
			ConditionalEventDefinition conditionalEventDef = parseConditionalStartEventForEventSubprocess(conditionalEventDefinitionElement, startEventActivity, isInterrupting);
			behavior = new EventSubProcessStartConditionalEventActivityBehavior(conditionalEventDef);
		  }
		  else
		  {
			addError("start event of event subprocess must be of type 'error', 'message', 'timer', 'signal', 'compensation' or 'escalation'", startEventElement);
		  }

		 startEventActivity.ActivityBehavior = behavior;
		}
		else
		{ // "regular" subprocess
		  Element conditionalEventDefinition = startEventElement.element(CONDITIONAL_EVENT_DEFINITION);

		  if (conditionalEventDefinition != null)
		  {
			addError("conditionalEventDefinition is not allowed on start event within a subprocess", conditionalEventDefinition);
		  }
		  if (timerEventDefinition != null)
		  {
			addError("timerEventDefinition is not allowed on start event within a subprocess", timerEventDefinition);
		  }
		  if (escalationEventDefinitionElement != null)
		  {
			addError("escalationEventDefinition is not allowed on start event within a subprocess", escalationEventDefinitionElement);
		  }
		  if (compensateEventDefinition != null)
		  {
			addError("compensateEventDefinition is not allowed on start event within a subprocess", compensateEventDefinition);
		  }
		  if (errorEventDefinition != null)
		  {
			addError("errorEventDefinition only allowed on start event if subprocess is an event subprocess", errorEventDefinition);
		  }
		  if (messageEventDefinition != null)
		  {
			addError("messageEventDefinition only allowed on start event if subprocess is an event subprocess", messageEventDefinition);
		  }
		  if (signalEventDefinition != null)
		  {
			addError("signalEventDefintion only allowed on start event if subprocess is an event subprocess", messageEventDefinition);
		  }

		  startEventActivity.ActivityBehavior = new NoneStartEventActivityBehavior();
		}
	  }

	  protected internal virtual void parseCompensationEventSubprocess(ActivityImpl startEventActivity, Element startEventElement, ActivityImpl scopeActivity, Element compensateEventDefinition)
	  {
		startEventActivity.Properties.set(BpmnProperties.TYPE, ActivityTypes.START_EVENT_COMPENSATION);
		scopeActivity.setProperty(PROPERTYNAME_IS_FOR_COMPENSATION, true);

		if (scopeActivity.FlowScope is ProcessDefinitionEntity)
		{
		  addError("event subprocess with compensation start event is only supported for embedded subprocess " + "(since throwing compensation through a call activity-induced process hierarchy is not supported)", startEventElement);
		}

		ScopeImpl subprocess = scopeActivity.FlowScope;
		ActivityImpl compensationHandler = ((ActivityImpl) subprocess).findCompensationHandler();
		if (compensationHandler == null)
		{
		  // add property to subprocess
		  subprocess.setProperty(PROPERTYNAME_COMPENSATION_HANDLER_ID, scopeActivity.ActivityId);
		}
		else
		{

		  if (compensationHandler.SubProcessScope)
		  {
			addError("multiple event subprocesses with compensation start event are not supported on the same scope", startEventElement);
		  }
		  else
		  {
			addError("compensation boundary event and event subprocess with compensation start event are not supported on the same scope", startEventElement);
		  }
		}

		validateCatchCompensateEventDefinition(compensateEventDefinition);
	  }

	  protected internal virtual void parseErrorStartEventDefinition(Element errorEventDefinition, ActivityImpl startEventActivity)
	  {
		startEventActivity.Properties.set(BpmnProperties.TYPE, ActivityTypes.START_EVENT_ERROR);
		string errorRef = errorEventDefinition.attribute("errorRef");
		Error error = null;
		// the error event definition executes the event subprocess activity which
		// hosts the start event
		string eventSubProcessActivity = startEventActivity.FlowScope.Id;
		ErrorEventDefinition definition = new ErrorEventDefinition(eventSubProcessActivity);
		if (!string.ReferenceEquals(errorRef, null))
		{
		  error = errors[errorRef];
		  string errorCode = error == null ? errorRef : error.ErrorCode;
		  definition.ErrorCode = errorCode;
		}
		definition.Precedence = 10;
		setErrorCodeVariableOnErrorEventDefinition(errorEventDefinition, definition);
		setErrorMessageVariableOnErrorEventDefinition(errorEventDefinition, definition);
		addErrorEventDefinition(definition, startEventActivity.EventScope);
	  }

	  /// <summary>
	  /// Sets the value for "camunda:errorCodeVariable" on the passed definition if
	  /// it's present.
	  /// </summary>
	  /// <param name="errorEventDefinition">
	  ///          the XML errorEventDefinition tag </param>
	  /// <param name="definition">
	  ///          the errorEventDefintion that can get the errorCodeVariable value </param>
	  protected internal virtual void setErrorCodeVariableOnErrorEventDefinition(Element errorEventDefinition, ErrorEventDefinition definition)
	  {
		string errorCodeVar = errorEventDefinition.attributeNS(CAMUNDA_BPMN_EXTENSIONS_NS, "errorCodeVariable");
		if (!string.ReferenceEquals(errorCodeVar, null))
		{
		  definition.ErrorCodeVariable = errorCodeVar;
		}
	  }

	  /// <summary>
	  /// Sets the value for "camunda:errorMessageVariable" on the passed definition if
	  /// it's present.
	  /// </summary>
	  /// <param name="errorEventDefinition">
	  ///          the XML errorEventDefinition tag </param>
	  /// <param name="definition">
	  ///          the errorEventDefintion that can get the errorMessageVariable value </param>
	  protected internal virtual void setErrorMessageVariableOnErrorEventDefinition(Element errorEventDefinition, ErrorEventDefinition definition)
	  {
		string errorMessageVariable = errorEventDefinition.attributeNS(CAMUNDA_BPMN_EXTENSIONS_NS, "errorMessageVariable");
		if (!string.ReferenceEquals(errorMessageVariable, null))
		{
		  definition.ErrorMessageVariable = errorMessageVariable;
		}
	  }

	  protected internal virtual EventSubscriptionDeclaration parseMessageEventDefinition(Element messageEventDefinition)
	  {
		string messageRef = messageEventDefinition.attribute("messageRef");
		if (string.ReferenceEquals(messageRef, null))
		{
		  addError("attribute 'messageRef' is required", messageEventDefinition);
		}
		MessageDefinition messageDefinition = messages[resolveName(messageRef)];
		if (messageDefinition == null)
		{
		  addError("Invalid 'messageRef': no message with id '" + messageRef + "' found.", messageEventDefinition);
		}
		return new EventSubscriptionDeclaration(messageDefinition.Expression, EventType.MESSAGE);
	  }

	  protected internal virtual void addEventSubscriptionDeclaration(EventSubscriptionDeclaration subscription, ScopeImpl scope, Element element)
	  {
		if (subscription.EventType.Equals(EventType.MESSAGE.name()) && (!subscription.hasEventName()))
		{
		  addError("Cannot have a message event subscription with an empty or missing name", element);
		}

		IDictionary<string, EventSubscriptionDeclaration> eventDefinitions = scope.Properties.get(BpmnProperties.EVENT_SUBSCRIPTION_DECLARATIONS);

		// if this is a message event, validate that it is the only one with the provided name for this scope
		if (hasMultipleMessageEventDefinitionsWithSameName(subscription, eventDefinitions.Values))
		{
		  addError("Cannot have more than one message event subscription with name '" + subscription.UnresolvedEventName + "' for scope '" + scope.Id + "'", element);
		}

		// if this is a signal event, validate that it is the only one with the provided name for this scope
		if (hasMultipleSignalEventDefinitionsWithSameName(subscription, eventDefinitions.Values))
		{
		  addError("Cannot have more than one signal event subscription with name '" + subscription.UnresolvedEventName + "' for scope '" + scope.Id + "'", element);
		}
		// if this is a conditional event, validate that it is the only one with the provided condition
		if (subscription.StartEvent && hasMultipleConditionalEventDefinitionsWithSameCondition(subscription, eventDefinitions.Values))
		{
		  addError("Cannot have more than one conditional event subscription with the same condition '" + ((ConditionalEventDefinition) subscription).ConditionAsString + "'", element);
		}

		scope.Properties.putMapEntry(BpmnProperties.EVENT_SUBSCRIPTION_DECLARATIONS, subscription.ActivityId, subscription);
	  }

	  protected internal virtual bool hasMultipleMessageEventDefinitionsWithSameName(EventSubscriptionDeclaration subscription, ICollection<EventSubscriptionDeclaration> eventDefinitions)
	  {
		return hasMultipleEventDefinitionsWithSameName(subscription, eventDefinitions, EventType.MESSAGE.name());
	  }

	  protected internal virtual bool hasMultipleSignalEventDefinitionsWithSameName(EventSubscriptionDeclaration subscription, ICollection<EventSubscriptionDeclaration> eventDefinitions)
	  {
		return hasMultipleEventDefinitionsWithSameName(subscription, eventDefinitions, EventType.SIGNAL.name());
	  }

	  protected internal virtual bool hasMultipleConditionalEventDefinitionsWithSameCondition(EventSubscriptionDeclaration subscription, ICollection<EventSubscriptionDeclaration> eventDefinitions)
	  {
		if (subscription.EventType.Equals(EventType.CONDITONAL.name()))
		{
		  foreach (EventSubscriptionDeclaration eventDefinition in eventDefinitions)
		  {
			if (eventDefinition.EventType.Equals(EventType.CONDITONAL.name()) && eventDefinition.StartEvent == subscription.StartEvent && ((ConditionalEventDefinition) eventDefinition).ConditionAsString.Equals(((ConditionalEventDefinition) subscription).ConditionAsString))
			{
			  return true;
			}
		  }
		}
		return false;
	  }

	  protected internal virtual bool hasMultipleEventDefinitionsWithSameName(EventSubscriptionDeclaration subscription, ICollection<EventSubscriptionDeclaration> eventDefinitions, string eventType)
	  {
		if (subscription.EventType.Equals(eventType))
		{
		  foreach (EventSubscriptionDeclaration eventDefinition in eventDefinitions)
		  {
			if (eventDefinition.EventType.Equals(eventType) && eventDefinition.UnresolvedEventName.Equals(subscription.UnresolvedEventName) && eventDefinition.StartEvent == subscription.StartEvent)
			{
			 return true;
			}
		  }
		}
		return false;
	  }

	  protected internal virtual void addEventSubscriptionJobDeclaration(EventSubscriptionJobDeclaration jobDeclaration, ActivityImpl activity, Element element)
	  {
		IList<EventSubscriptionJobDeclaration> jobDeclarationsForActivity = (IList<EventSubscriptionJobDeclaration>) activity.getProperty(PROPERTYNAME_EVENT_SUBSCRIPTION_JOB_DECLARATION);

		if (jobDeclarationsForActivity == null)
		{
		  jobDeclarationsForActivity = new List<EventSubscriptionJobDeclaration>();
		  activity.setProperty(PROPERTYNAME_EVENT_SUBSCRIPTION_JOB_DECLARATION, jobDeclarationsForActivity);
		}

		if (activityAlreadyContainsJobDeclarationEventType(jobDeclarationsForActivity, jobDeclaration))
		{
		  addError("Activity contains already job declaration with type " + jobDeclaration.EventType, element);
		}

		jobDeclarationsForActivity.Add(jobDeclaration);
	  }

	  /// <summary>
	  /// Assumes that an activity has at most one declaration of a certain eventType.
	  /// </summary>
	  protected internal virtual bool activityAlreadyContainsJobDeclarationEventType(IList<EventSubscriptionJobDeclaration> jobDeclarationsForActivity, EventSubscriptionJobDeclaration jobDeclaration)
	  {
		foreach (EventSubscriptionJobDeclaration declaration in jobDeclarationsForActivity)
		{
		  if (declaration.EventType.Equals(jobDeclaration.EventType))
		  {
			return true;
		  }
		}
		return false;
	  }

	  /// <summary>
	  /// Parses the activities of a certain level in the process (process,
	  /// subprocess or another scope).
	  /// </summary>
	  /// <param name="activityElements">
	  ///          The list of activities to be parsed. This list may be filtered before. </param>
	  /// <param name="parentElement">
	  ///          The 'parent' element that contains the activities (process, subprocess). </param>
	  /// <param name="scopeElement">
	  ///          The <seealso cref="ScopeImpl"/> to which the activities must be added. </param>
	  public virtual void parseActivities(IList<Element> activityElements, Element parentElement, ScopeImpl scopeElement)
	  {
		foreach (Element activityElement in activityElements)
		{
		  parseActivity(activityElement, parentElement, scopeElement);
		}
	  }

	  protected internal virtual ActivityImpl parseActivity(Element activityElement, Element parentElement, ScopeImpl scopeElement)
	  {
		ActivityImpl activity = null;

		bool isMultiInstance = false;
		ScopeImpl miBody = parseMultiInstanceLoopCharacteristics(activityElement, scopeElement);
		if (miBody != null)
		{
		  scopeElement = miBody;
		  isMultiInstance = true;
		}

		if (activityElement.TagName.Equals(ActivityTypes.GATEWAY_EXCLUSIVE))
		{
		  activity = parseExclusiveGateway(activityElement, scopeElement);
		}
		else if (activityElement.TagName.Equals(ActivityTypes.GATEWAY_INCLUSIVE))
		{
		  activity = parseInclusiveGateway(activityElement, scopeElement);
		}
		else if (activityElement.TagName.Equals(ActivityTypes.GATEWAY_PARALLEL))
		{
		  activity = parseParallelGateway(activityElement, scopeElement);
		}
		else if (activityElement.TagName.Equals(ActivityTypes.TASK_SCRIPT))
		{
		  activity = parseScriptTask(activityElement, scopeElement);
		}
		else if (activityElement.TagName.Equals(ActivityTypes.TASK_SERVICE))
		{
		  activity = parseServiceTask(activityElement, scopeElement);
		}
		else if (activityElement.TagName.Equals(ActivityTypes.TASK_BUSINESS_RULE))
		{
		  activity = parseBusinessRuleTask(activityElement, scopeElement);
		}
		else if (activityElement.TagName.Equals(ActivityTypes.TASK))
		{
		  activity = parseTask(activityElement, scopeElement);
		}
		else if (activityElement.TagName.Equals(ActivityTypes.TASK_MANUAL_TASK))
		{
		  activity = parseManualTask(activityElement, scopeElement);
		}
		else if (activityElement.TagName.Equals(ActivityTypes.TASK_USER_TASK))
		{
		  activity = parseUserTask(activityElement, scopeElement);
		}
		else if (activityElement.TagName.Equals(ActivityTypes.TASK_SEND_TASK))
		{
		  activity = parseSendTask(activityElement, scopeElement);
		}
		else if (activityElement.TagName.Equals(ActivityTypes.TASK_RECEIVE_TASK))
		{
		  activity = parseReceiveTask(activityElement, scopeElement);
		}
		else if (activityElement.TagName.Equals(ActivityTypes.SUB_PROCESS))
		{
		  activity = parseSubProcess(activityElement, scopeElement);
		}
		else if (activityElement.TagName.Equals(ActivityTypes.CALL_ACTIVITY))
		{
		  activity = parseCallActivity(activityElement, scopeElement, isMultiInstance);
		}
		else if (activityElement.TagName.Equals(ActivityTypes.INTERMEDIATE_EVENT_THROW))
		{
		  activity = parseIntermediateThrowEvent(activityElement, scopeElement);
		}
		else if (activityElement.TagName.Equals(ActivityTypes.GATEWAY_EVENT_BASED))
		{
		  activity = parseEventBasedGateway(activityElement, parentElement, scopeElement);
		}
		else if (activityElement.TagName.Equals(ActivityTypes.TRANSACTION))
		{
		  activity = parseTransaction(activityElement, scopeElement);
		}
		else if (activityElement.TagName.Equals(ActivityTypes.SUB_PROCESS_AD_HOC) || activityElement.TagName.Equals(ActivityTypes.GATEWAY_COMPLEX))
		{
		  addWarning("Ignoring unsupported activity type", activityElement);
		}

		if (isMultiInstance)
		{
		  activity.setProperty(PROPERTYNAME_IS_MULTI_INSTANCE, true);
		}

		if (activity != null)
		{
		  activity.Name = activityElement.attribute("name");
		  parseActivityInputOutput(activityElement, activity);
		}

		return activity;
	  }

	  public virtual void validateActivities(IList<ActivityImpl> activities)
	  {
		foreach (ActivityImpl activity in activities)
		{
		  validateActivity(activity);
		  // check children if it is an own scope / subprocess / ...
		  if (activity.Activities.Count > 0)
		  {
			validateActivities(activity.Activities);
		  }
		}
	  }

	  protected internal virtual void validateActivity(ActivityImpl activity)
	  {
		if (activity.ActivityBehavior is ExclusiveGatewayActivityBehavior)
		{
		  validateExclusiveGateway(activity);
		}
		validateOutgoingFlows(activity);
	  }

	  protected internal virtual void validateOutgoingFlows(ActivityImpl activity)
	  {
		if (activity.AsyncAfter)
		{
		  foreach (PvmTransition transition in activity.OutgoingTransitions)
		  {
			if (string.ReferenceEquals(transition.Id, null))
			{
			  addError("Sequence flow with sourceRef='" + activity.Id + "' must have an id, activity with id '" + activity.Id + "' uses 'asyncAfter'.", null);
			}
		  }
		}
	  }

	  public virtual void validateExclusiveGateway(ActivityImpl activity)
	  {
		if (activity.OutgoingTransitions.Count == 0)
		{
		  // TODO: double check if this is valid (I think in Activiti yes, since we
		  // need start events we will need an end event as well)
		  addError("Exclusive Gateway '" + activity.Id + "' has no outgoing sequence flows.", null);
		}
		else if (activity.OutgoingTransitions.Count == 1)
		{
		  PvmTransition flow = activity.OutgoingTransitions[0];
		  Condition condition = (Condition) flow.getProperty(BpmnParse.PROPERTYNAME_CONDITION);
		  if (condition != null)
		  {
			addError("Exclusive Gateway '" + activity.Id + "' has only one outgoing sequence flow ('" + flow.Id + "'). This is not allowed to have a condition.", null);
		  }
		}
		else
		{
		  string defaultSequenceFlow = (string) activity.getProperty("default");
		  bool hasDefaultFlow = !string.ReferenceEquals(defaultSequenceFlow, null) && defaultSequenceFlow.Length > 0;

		  List<PvmTransition> flowsWithoutCondition = new List<PvmTransition>();
		  foreach (PvmTransition flow in activity.OutgoingTransitions)
		  {
			Condition condition = (Condition) flow.getProperty(BpmnParse.PROPERTYNAME_CONDITION);
			bool isDefaultFlow = !string.ReferenceEquals(flow.Id, null) && flow.Id.Equals(defaultSequenceFlow);
			bool hasConditon = condition != null;

			if (!hasConditon && !isDefaultFlow)
			{
			  flowsWithoutCondition.Add(flow);
			}
			if (hasConditon && isDefaultFlow)
			{
			  addError("Exclusive Gateway '" + activity.Id + "' has outgoing sequence flow '" + flow.Id + "' which is the default flow but has a condition too.", null);
			}
		  }
		  if (hasDefaultFlow || flowsWithoutCondition.Count > 1)
		  {
			// if we either have a default flow (then no flows without conditions
			// are valid at all) or if we have more than one flow without condition
			// this is an error
			foreach (PvmTransition flow in flowsWithoutCondition)
			{
			  addError("Exclusive Gateway '" + activity.Id + "' has outgoing sequence flow '" + flow.Id + "' without condition which is not the default flow.", null);
			}
		  }
		  else if (flowsWithoutCondition.Count == 1)
		  {
			// Havinf no default and exactly one flow without condition this is
			// considered the default one now (to not break backward compatibility)
			PvmTransition flow = flowsWithoutCondition[0];
			addWarning("Exclusive Gateway '" + activity.Id + "' has outgoing sequence flow '" + flow.Id + "' without condition which is not the default flow. We assume it to be the default flow, but it is bad modeling practice, better set the default flow in your gateway.", null);
		  }
		}
	  }

	  public virtual ActivityImpl parseIntermediateCatchEvent(Element intermediateEventElement, ScopeImpl scopeElement, ActivityImpl eventBasedGateway)
	  {
		ActivityImpl nestedActivity = createActivityOnScope(intermediateEventElement, scopeElement);

		Element timerEventDefinition = intermediateEventElement.element(TIMER_EVENT_DEFINITION);
		Element signalEventDefinition = intermediateEventElement.element(SIGNAL_EVENT_DEFINITION);
		Element messageEventDefinition = intermediateEventElement.element(MESSAGE_EVENT_DEFINITION);
		Element linkEventDefinitionElement = intermediateEventElement.element(LINK_EVENT_DEFINITION);
		Element conditionalEventDefinitionElement = intermediateEventElement.element(CONDITIONAL_EVENT_DEFINITION);

		// shared by all events except for link event
		IntermediateCatchEventActivityBehavior defaultCatchBehaviour = new IntermediateCatchEventActivityBehavior(eventBasedGateway != null);

		parseAsynchronousContinuationForActivity(intermediateEventElement, nestedActivity);
		bool isEventBaseGatewayPresent = eventBasedGateway != null;

		if (isEventBaseGatewayPresent)
		{
		  nestedActivity.EventScope = eventBasedGateway;
		  nestedActivity.ActivityStartBehavior = ActivityStartBehavior.CANCEL_EVENT_SCOPE;
		}
		else
		{
		  nestedActivity.EventScope = nestedActivity;
		  nestedActivity.Scope = true;
		}

		nestedActivity.ActivityBehavior = defaultCatchBehaviour;
		if (timerEventDefinition != null)
		{
		  parseIntermediateTimerEventDefinition(timerEventDefinition, nestedActivity);

		}
		else if (signalEventDefinition != null)
		{
		  parseIntermediateSignalEventDefinition(signalEventDefinition, nestedActivity);

		}
		else if (messageEventDefinition != null)
		{
		  parseIntermediateMessageEventDefinition(messageEventDefinition, nestedActivity);

		}
		else if (linkEventDefinitionElement != null)
		{
		  if (isEventBaseGatewayPresent)
		  {
			addError("IntermediateCatchLinkEvent is not allowed after an EventBasedGateway.", intermediateEventElement);
		  }
		  nestedActivity.ActivityBehavior = new IntermediateCatchLinkEventActivityBehavior();
		  parseIntermediateLinkEventCatchBehavior(intermediateEventElement, nestedActivity, linkEventDefinitionElement);

		}
		else if (conditionalEventDefinitionElement != null)
		{
		  ConditionalEventDefinition conditionalEvent = parseIntermediateConditionalEventDefinition(conditionalEventDefinitionElement, nestedActivity);
		  nestedActivity.ActivityBehavior = new IntermediateConditionalEventBehavior(conditionalEvent, isEventBaseGatewayPresent);
		}
		else
		{
		  addError("Unsupported intermediate catch event type", intermediateEventElement);
		}

		parseExecutionListenersOnScope(intermediateEventElement, nestedActivity);

		foreach (BpmnParseListener parseListener in parseListeners)
		{
		  parseListener.parseIntermediateCatchEvent(intermediateEventElement, scopeElement, nestedActivity);
		}

		return nestedActivity;
	  }

	  protected internal virtual void parseIntermediateLinkEventCatchBehavior(Element intermediateEventElement, ActivityImpl activity, Element linkEventDefinitionElement)
	  {

		activity.Properties.set(BpmnProperties.TYPE, ActivityTypes.INTERMEDIATE_EVENT_LINK);

		string linkName = linkEventDefinitionElement.attribute("name");
		string elementName = intermediateEventElement.attribute("name");
		string elementId = intermediateEventElement.attribute("id");

		if (eventLinkTargets.ContainsKey(linkName))
		{
		  addError("Multiple Intermediate Catch Events with the same link event name ('" + linkName + "') are not allowed.", intermediateEventElement);
		}
		else
		{
		  if (!linkName.Equals(elementName))
		  {
			// this is valid - but not a good practice (as it is really confusing
			// for the reader of the process model) - hence we log a warning
			addWarning("Link Event named '" + elementName + "' containes link event definition with name '" + linkName + "' - it is recommended to use the same name for both.", intermediateEventElement);
		  }

		  // now we remember the link in order to replace the sequence flow later on
		  eventLinkTargets[linkName] = elementId;
		}
	  }

	  protected internal virtual void parseIntermediateMessageEventDefinition(Element messageEventDefinition, ActivityImpl nestedActivity)
	  {

		nestedActivity.Properties.set(BpmnProperties.TYPE, ActivityTypes.INTERMEDIATE_EVENT_MESSAGE);

		EventSubscriptionDeclaration messageDefinition = parseMessageEventDefinition(messageEventDefinition);
		messageDefinition.ActivityId = nestedActivity.Id;
		addEventSubscriptionDeclaration(messageDefinition, nestedActivity.EventScope, messageEventDefinition);

		foreach (BpmnParseListener parseListener in parseListeners)
		{
		  parseListener.parseIntermediateMessageCatchEventDefinition(messageEventDefinition, nestedActivity);
		}
	  }

	  public virtual ActivityImpl parseIntermediateThrowEvent(Element intermediateEventElement, ScopeImpl scopeElement)
	  {
		Element signalEventDefinitionElement = intermediateEventElement.element(SIGNAL_EVENT_DEFINITION);
		Element compensateEventDefinitionElement = intermediateEventElement.element(COMPENSATE_EVENT_DEFINITION);
		Element linkEventDefinitionElement = intermediateEventElement.element(LINK_EVENT_DEFINITION);
		Element messageEventDefinitionElement = intermediateEventElement.element(MESSAGE_EVENT_DEFINITION);
		Element escalationEventDefinition = intermediateEventElement.element(ESCALATION_EVENT_DEFINITION);

		// the link event gets a special treatment as a throwing link event (event
		// source)
		// will not create any activity instance but serves as a "redirection" to
		// the catching link
		// event (event target)
		if (linkEventDefinitionElement != null)
		{
		  string linkName = linkEventDefinitionElement.attribute("name");
		  string elementId = intermediateEventElement.attribute("id");

		  // now we remember the link in order to replace the sequence flow later on
		  eventLinkSources[elementId] = linkName;
		  // and done - no activity created
		  return null;
		}

		ActivityImpl nestedActivityImpl = createActivityOnScope(intermediateEventElement, scopeElement);
		ActivityBehavior activityBehavior = null;

		parseAsynchronousContinuationForActivity(intermediateEventElement, nestedActivityImpl);

		if (signalEventDefinitionElement != null)
		{
		  nestedActivityImpl.Properties.set(BpmnProperties.TYPE, ActivityTypes.INTERMEDIATE_EVENT_SIGNAL_THROW);

		  EventSubscriptionDeclaration signalDefinition = parseSignalEventDefinition(signalEventDefinitionElement, true);
		  activityBehavior = new ThrowSignalEventActivityBehavior(signalDefinition);
		}
		else if (compensateEventDefinitionElement != null)
		{
		  nestedActivityImpl.Properties.set(BpmnProperties.TYPE, ActivityTypes.INTERMEDIATE_EVENT_COMPENSATION_THROW);
		  CompensateEventDefinition compensateEventDefinition = parseThrowCompensateEventDefinition(compensateEventDefinitionElement, scopeElement);
		  activityBehavior = new CompensationEventActivityBehavior(compensateEventDefinition);
		  nestedActivityImpl.setProperty(PROPERTYNAME_THROWS_COMPENSATION, true);
		  nestedActivityImpl.Scope = true;
		}
		else if (messageEventDefinitionElement != null)
		{
		  if (isServiceTaskLike(messageEventDefinitionElement))
		  {

			// CAM-436 same behavior as service task
			nestedActivityImpl.Properties.set(BpmnProperties.TYPE, ActivityTypes.INTERMEDIATE_EVENT_MESSAGE_THROW);
			activityBehavior = parseServiceTaskLike(ActivityTypes.INTERMEDIATE_EVENT_MESSAGE_THROW, messageEventDefinitionElement, scopeElement).ActivityBehavior;
		  }
		  else
		  {
			// default to non behavior if no service task
			// properties have been specified
			nestedActivityImpl.Properties.set(BpmnProperties.TYPE, ActivityTypes.INTERMEDIATE_EVENT_NONE_THROW);
			activityBehavior = new IntermediateThrowNoneEventActivityBehavior();
		  }
		}
		else if (escalationEventDefinition != null)
		{
		  nestedActivityImpl.Properties.set(BpmnProperties.TYPE, ActivityTypes.INTERMEDIATE_EVENT_ESCALATION_THROW);

		  Escalation escalation = findEscalationForEscalationEventDefinition(escalationEventDefinition);
		  if (escalation != null && string.ReferenceEquals(escalation.EscalationCode, null))
		  {
			addError("throwing escalation event must have an 'escalationCode'", escalationEventDefinition);
		  }

		  activityBehavior = new ThrowEscalationEventActivityBehavior(escalation);

		}
		else
		{ // None intermediate event
		  nestedActivityImpl.Properties.set(BpmnProperties.TYPE, ActivityTypes.INTERMEDIATE_EVENT_NONE_THROW);
		  activityBehavior = new IntermediateThrowNoneEventActivityBehavior();
		}

		nestedActivityImpl.ActivityBehavior = activityBehavior;

		parseExecutionListenersOnScope(intermediateEventElement, nestedActivityImpl);

		foreach (BpmnParseListener parseListener in parseListeners)
		{
		  parseListener.parseIntermediateThrowEvent(intermediateEventElement, scopeElement, nestedActivityImpl);
		}

		return nestedActivityImpl;
	  }

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: protected CompensateEventDefinition parseThrowCompensateEventDefinition(final org.camunda.bpm.engine.impl.util.xml.Element compensateEventDefinitionElement, ScopeImpl scopeElement)
	  protected internal virtual CompensateEventDefinition parseThrowCompensateEventDefinition(Element compensateEventDefinitionElement, ScopeImpl scopeElement)
	  {
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final String activityRef = compensateEventDefinitionElement.attribute("activityRef");
		string activityRef = compensateEventDefinitionElement.attribute("activityRef");
		bool waitForCompletion = TRUE.Equals(compensateEventDefinitionElement.attribute("waitForCompletion", TRUE));

		if (!string.ReferenceEquals(activityRef, null))
		{
		  if (scopeElement.findActivityAtLevelOfSubprocess(activityRef) == null)
		  {
			bool? isTriggeredByEvent = scopeElement.Properties.get(BpmnProperties.TRIGGERED_BY_EVENT);
			string type = (string) scopeElement.getProperty(PROPERTYNAME_TYPE);
			if (true == isTriggeredByEvent && "subProcess".Equals(type))
			{
			  scopeElement = scopeElement.FlowScope;
			}
			if (scopeElement.findActivityAtLevelOfSubprocess(activityRef) == null)
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final String scopeId = scopeElement.getId();
			  string scopeId = scopeElement.Id;
			  scopeElement.addToBacklog(activityRef, new BacklogErrorCallbackAnonymousInnerClass(this, compensateEventDefinitionElement, activityRef, scopeId));
			}
		  }
		}

		CompensateEventDefinition compensateEventDefinition = new CompensateEventDefinition();
		compensateEventDefinition.ActivityRef = activityRef;

		compensateEventDefinition.WaitForCompletion = waitForCompletion;
		if (!waitForCompletion)
		{
		  addWarning("Unsupported attribute value for 'waitForCompletion': 'waitForCompletion=false' is not supported. Compensation event will wait for compensation to join.", compensateEventDefinitionElement);
		}

		return compensateEventDefinition;
	  }

	  private class BacklogErrorCallbackAnonymousInnerClass : ScopeImpl.BacklogErrorCallback
	  {
		  private readonly BpmnParse outerInstance;

		  private Element compensateEventDefinitionElement;
		  private string activityRef;
		  private string scopeId;

		  public BacklogErrorCallbackAnonymousInnerClass(BpmnParse outerInstance, Element compensateEventDefinitionElement, string activityRef, string scopeId)
		  {
			  this.outerInstance = outerInstance;
			  this.compensateEventDefinitionElement = compensateEventDefinitionElement;
			  this.activityRef = activityRef;
			  this.scopeId = scopeId;
		  }


		  public void callback()
		  {
			outerInstance.addError("Invalid attribute value for 'activityRef': no activity with id '" + activityRef + "' in scope '" + scopeId + "'", compensateEventDefinitionElement);
		  }
	  }

	  protected internal virtual void validateCatchCompensateEventDefinition(Element compensateEventDefinitionElement)
	  {
		string activityRef = compensateEventDefinitionElement.attribute("activityRef");
		if (!string.ReferenceEquals(activityRef, null))
		{
		  addWarning("attribute 'activityRef' is not supported on catching compensation event. attribute will be ignored", compensateEventDefinitionElement);
		}

		string waitForCompletion = compensateEventDefinitionElement.attribute("waitForCompletion");
		if (!string.ReferenceEquals(waitForCompletion, null))
		{
		  addWarning("attribute 'waitForCompletion' is not supported on catching compensation event. attribute will be ignored", compensateEventDefinitionElement);
		}
	  }

	  protected internal virtual void parseBoundaryCompensateEventDefinition(Element compensateEventDefinition, ActivityImpl activity)
	  {
		activity.Properties.set(BpmnProperties.TYPE, ActivityTypes.BOUNDARY_COMPENSATION);

		ScopeImpl hostActivity = activity.EventScope;
		foreach (ActivityImpl sibling in activity.FlowScope.Activities)
		{
		  if (sibling.getProperty(BpmnProperties.TYPE.Name).Equals("compensationBoundaryCatch") && sibling.EventScope.Equals(hostActivity) && sibling != activity)
		  {
			addError("multiple boundary events with compensateEventDefinition not supported on same activity", compensateEventDefinition);
		  }
		}

		validateCatchCompensateEventDefinition(compensateEventDefinition);
	  }

	  protected internal virtual ActivityBehavior parseBoundaryCancelEventDefinition(Element cancelEventDefinition, ActivityImpl activity)
	  {
		activity.Properties.set(BpmnProperties.TYPE, ActivityTypes.BOUNDARY_CANCEL);

		LegacyBehavior.parseCancelBoundaryEvent(activity);

		ActivityImpl transaction = (ActivityImpl) activity.EventScope;
		if (transaction.ActivityBehavior != null && transaction.ActivityBehavior is MultiInstanceActivityBehavior)
		{
		  transaction = transaction.Activities[0];
		}

		if (!"transaction".Equals(transaction.getProperty(BpmnProperties.TYPE.Name)))
		{
		  addError("boundary event with cancelEventDefinition only supported on transaction subprocesses", cancelEventDefinition);
		}

		// ensure there is only one cancel boundary event
		foreach (ActivityImpl sibling in activity.FlowScope.Activities)
		{
		  if ("cancelBoundaryCatch".Equals(sibling.getProperty(BpmnProperties.TYPE.Name)) && sibling != activity && sibling.EventScope == transaction)
		  {
			addError("multiple boundary events with cancelEventDefinition not supported on same transaction subprocess", cancelEventDefinition);
		  }
		}

		// find all cancel end events
		foreach (ActivityImpl childActivity in transaction.Activities)
		{
		  ActivityBehavior activityBehavior = childActivity.ActivityBehavior;
		  if (activityBehavior != null && activityBehavior is CancelEndEventActivityBehavior)
		  {
			((CancelEndEventActivityBehavior) activityBehavior).CancelBoundaryEvent = activity;
		  }
		}

		return new CancelBoundaryEventActivityBehavior();
	  }

	  /// <summary>
	  /// Parses loopCharacteristics (standardLoop/Multi-instance) of an activity, if
	  /// any is defined.
	  /// </summary>
	  public virtual ScopeImpl parseMultiInstanceLoopCharacteristics(Element activityElement, ScopeImpl scope)
	  {

		Element miLoopCharacteristics = activityElement.element("multiInstanceLoopCharacteristics");
		if (miLoopCharacteristics == null)
		{
		  return null;
		}
		else
		{
		  string id = activityElement.attribute("id");

		  LOG.parsingElement("mi body for activity", id);

		  id = getIdForMiBody(id);
		  ActivityImpl miBodyScope = scope.createActivity(id);
		  ActivityAsyncDelegates = miBodyScope;
		  miBodyScope.setProperty(PROPERTYNAME_TYPE, ActivityTypes.MULTI_INSTANCE_BODY);
		  miBodyScope.Scope = true;

		  bool isSequential = parseBooleanAttribute(miLoopCharacteristics.attribute("isSequential"), false).Value;

		  MultiInstanceActivityBehavior behavior = null;
		  if (isSequential)
		  {
			behavior = new SequentialMultiInstanceActivityBehavior();
		  }
		  else
		  {
			behavior = new ParallelMultiInstanceActivityBehavior();
		  }
		  miBodyScope.ActivityBehavior = behavior;

		  // loopCardinality
		  Element loopCardinality = miLoopCharacteristics.element("loopCardinality");
		  if (loopCardinality != null)
		  {
			string loopCardinalityText = loopCardinality.Text;
			if (string.ReferenceEquals(loopCardinalityText, null) || "".Equals(loopCardinalityText))
			{
			  addError("loopCardinality must be defined for a multiInstanceLoopCharacteristics definition ", miLoopCharacteristics);
			}
			behavior.LoopCardinalityExpression = expressionManager.createExpression(loopCardinalityText);
		  }

		  // completionCondition
		  Element completionCondition = miLoopCharacteristics.element("completionCondition");
		  if (completionCondition != null)
		  {
			string completionConditionText = completionCondition.Text;
			behavior.CompletionConditionExpression = expressionManager.createExpression(completionConditionText);
		  }

		  // activiti:collection
		  string collection = miLoopCharacteristics.attributeNS(CAMUNDA_BPMN_EXTENSIONS_NS, "collection");
		  if (!string.ReferenceEquals(collection, null))
		  {
			if (collection.Contains("{"))
			{
			  behavior.CollectionExpression = expressionManager.createExpression(collection);
			}
			else
			{
			  behavior.CollectionVariable = collection;
			}
		  }

		  // loopDataInputRef
		  Element loopDataInputRef = miLoopCharacteristics.element("loopDataInputRef");
		  if (loopDataInputRef != null)
		  {
			string loopDataInputRefText = loopDataInputRef.Text;
			if (!string.ReferenceEquals(loopDataInputRefText, null))
			{
			  if (loopDataInputRefText.Contains("{"))
			  {
				behavior.CollectionExpression = expressionManager.createExpression(loopDataInputRefText);
			  }
			  else
			  {
				behavior.CollectionVariable = loopDataInputRefText;
			  }
			}
		  }

		  // activiti:elementVariable
		  string elementVariable = miLoopCharacteristics.attributeNS(CAMUNDA_BPMN_EXTENSIONS_NS, "elementVariable");
		  if (!string.ReferenceEquals(elementVariable, null))
		  {
			behavior.CollectionElementVariable = elementVariable;
		  }

		  // dataInputItem
		  Element inputDataItem = miLoopCharacteristics.element("inputDataItem");
		  if (inputDataItem != null)
		  {
			string inputDataItemName = inputDataItem.attribute("name");
			behavior.CollectionElementVariable = inputDataItemName;
		  }

		  // Validation
		  if (behavior.LoopCardinalityExpression == null && behavior.CollectionExpression == null && string.ReferenceEquals(behavior.CollectionVariable, null))
		  {
			addError("Either loopCardinality or loopDataInputRef/activiti:collection must been set", miLoopCharacteristics);
		  }

		  // Validation
		  if (behavior.CollectionExpression == null && string.ReferenceEquals(behavior.CollectionVariable, null) && !string.ReferenceEquals(behavior.CollectionElementVariable, null))
		  {
			addError("LoopDataInputRef/activiti:collection must be set when using inputDataItem or activiti:elementVariable", miLoopCharacteristics);
		  }

		  foreach (BpmnParseListener parseListener in parseListeners)
		  {
			parseListener.parseMultiInstanceLoopCharacteristics(activityElement, miLoopCharacteristics, miBodyScope);
		  }

		  return miBodyScope;
		}
	  }

	  public static string getIdForMiBody(string id)
	  {
		return id + MULTI_INSTANCE_BODY_ID_SUFFIX;
	  }

	  /// <summary>
	  /// Parses the generic information of an activity element (id, name,
	  /// documentation, etc.), and creates a new <seealso cref="ActivityImpl"/> on the given
	  /// scope element.
	  /// </summary>
	  public virtual ActivityImpl createActivityOnScope(Element activityElement, ScopeImpl scopeElement)
	  {
		string id = activityElement.attribute("id");

		LOG.parsingElement("activity", id);
		ActivityImpl activity = scopeElement.createActivity(id);

		activity.setProperty("name", activityElement.attribute("name"));
		activity.setProperty("documentation", parseDocumentation(activityElement));
		activity.setProperty("default", activityElement.attribute("default"));
		activity.Properties.set(BpmnProperties.TYPE, activityElement.TagName);
		activity.setProperty("line", activityElement.Line);
		ActivityAsyncDelegates = activity;
		activity.setProperty(PROPERTYNAME_JOB_PRIORITY, parsePriority(activityElement, PROPERTYNAME_JOB_PRIORITY));

		if (isCompensationHandler(activityElement))
		{
		  activity.setProperty(PROPERTYNAME_IS_FOR_COMPENSATION, true);
		}

		return activity;
	  }

	  /// <summary>
	  /// Sets the delegates for the activity, which will be called
	  /// if the attribute asyncAfter or asyncBefore was changed.
	  /// </summary>
	  /// <param name="activity"> the activity which gets the delegates </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: protected void setActivityAsyncDelegates(final ActivityImpl activity)
	  protected internal virtual ActivityImpl ActivityAsyncDelegates
	  {
		  set
		  {
			value.DelegateAsyncAfterUpdate = new AsyncAfterUpdateAnonymousInnerClass(this, value);
    
			value.DelegateAsyncBeforeUpdate = new AsyncBeforeUpdateAnonymousInnerClass(this, value);
		  }
	  }

	  private class AsyncAfterUpdateAnonymousInnerClass : ActivityImpl.AsyncAfterUpdate
	  {
		  private readonly BpmnParse outerInstance;

		  private org.camunda.bpm.engine.impl.pvm.process.ActivityImpl activity;

		  public AsyncAfterUpdateAnonymousInnerClass(BpmnParse outerInstance, org.camunda.bpm.engine.impl.pvm.process.ActivityImpl activity)
		  {
			  this.outerInstance = outerInstance;
			  this.activity = activity;
		  }

		  public void updateAsyncAfter(bool asyncAfter, bool exclusive)
		  {
			if (asyncAfter)
			{
			  outerInstance.addMessageJobDeclaration(new AsyncAfterMessageJobDeclaration(), activity, exclusive);
			}
			else
			{
			  outerInstance.removeMessageJobDeclarationWithJobConfiguration(activity, MessageJobDeclaration.ASYNC_AFTER);
			}
		  }
	  }

	  private class AsyncBeforeUpdateAnonymousInnerClass : ActivityImpl.AsyncBeforeUpdate
	  {
		  private readonly BpmnParse outerInstance;

		  private org.camunda.bpm.engine.impl.pvm.process.ActivityImpl activity;

		  public AsyncBeforeUpdateAnonymousInnerClass(BpmnParse outerInstance, org.camunda.bpm.engine.impl.pvm.process.ActivityImpl activity)
		  {
			  this.outerInstance = outerInstance;
			  this.activity = activity;
		  }

		  public void updateAsyncBefore(bool asyncBefore, bool exclusive)
		  {
			if (asyncBefore)
			{
			  outerInstance.addMessageJobDeclaration(new AsyncBeforeMessageJobDeclaration(), activity, exclusive);
			}
			else
			{
			  outerInstance.removeMessageJobDeclarationWithJobConfiguration(activity, MessageJobDeclaration.ASYNC_BEFORE);
			}
		  }
	  }

	  /// <summary>
	  /// Adds the new message job declaration to existing declarations.
	  /// There will be executed an existing check before the adding is executed.
	  /// </summary>
	  /// <param name="messageJobDeclaration"> the new message job declaration </param>
	  /// <param name="activity"> the corresponding activity </param>
	  /// <param name="exclusive"> the flag which indicates if the async should be exclusive </param>
	  protected internal virtual void addMessageJobDeclaration(MessageJobDeclaration messageJobDeclaration, ActivityImpl activity, bool exclusive)
	  {
		ProcessDefinition procDef = (ProcessDefinition) activity.ProcessDefinition;
		if (!exists(messageJobDeclaration, procDef.Key, activity.ActivityId))
		{
		  messageJobDeclaration.Exclusive = exclusive;
		  messageJobDeclaration.Activity = activity;
		  messageJobDeclaration.JobPriorityProvider = (ParameterValueProvider) activity.getProperty(PROPERTYNAME_JOB_PRIORITY);

		  addMessageJobDeclarationToActivity(messageJobDeclaration, activity);
		  addJobDeclarationToProcessDefinition(messageJobDeclaration, procDef);
		}
	  }

	  /// <summary>
	  /// Checks whether the message declaration already exists.
	  /// </summary>
	  /// <param name="msgJobdecl"> the message job declaration which is searched </param>
	  /// <param name="procDefKey"> the corresponding process definition key </param>
	  /// <param name="activityId"> the corresponding activity id </param>
	  /// <returns> true if the message job declaration exists, false otherwise </returns>
	  protected internal virtual bool exists(MessageJobDeclaration msgJobdecl, string procDefKey, string activityId)
	  {
		bool exist = false;
//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: List<JobDeclaration<?, ?>> declarations = jobDeclarations.get(procDefKey);
		IList<JobDeclaration<object, ?>> declarations = jobDeclarations[procDefKey];
		if (declarations != null)
		{
		  for (int i = 0; i < declarations.Count && !exist; i++)
		  {
//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: JobDeclaration<?, ?> decl = declarations.get(i);
			JobDeclaration<object, ?> decl = declarations[i];
			if (decl.ActivityId.Equals(activityId) && decl.JobConfiguration.Equals(msgJobdecl.JobConfiguration, StringComparison.OrdinalIgnoreCase))
			{
			  exist = true;
			}
		  }
		}
		return exist;
	  }

	  /// <summary>
	  /// Removes a job declaration which belongs to the given activity and has the given job configuration.
	  /// </summary>
	  /// <param name="activity"> the activity of the job declaration </param>
	  /// <param name="jobConfiguration">  the job configuration of the declaration </param>
	  protected internal virtual void removeMessageJobDeclarationWithJobConfiguration(ActivityImpl activity, string jobConfiguration)
	  {
		IList<MessageJobDeclaration> messageJobDeclarations = (IList<MessageJobDeclaration>) activity.getProperty(PROPERTYNAME_MESSAGE_JOB_DECLARATION);
		if (messageJobDeclarations != null)
		{
		  IEnumerator<MessageJobDeclaration> iter = messageJobDeclarations.GetEnumerator();
		  while (iter.MoveNext())
		  {
			MessageJobDeclaration msgDecl = iter.Current;
			if (msgDecl.JobConfiguration.Equals(jobConfiguration, StringComparison.OrdinalIgnoreCase) && msgDecl.ActivityId.Equals(activity.ActivityId, StringComparison.OrdinalIgnoreCase))
			{
//JAVA TO C# CONVERTER TODO TASK: .NET enumerators are read-only:
			  iter.remove();
			}
		  }
		}

		ProcessDefinition procDef = (ProcessDefinition) activity.ProcessDefinition;
//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: List<JobDeclaration<?, ?>> declarations = jobDeclarations.get(procDef.getKey());
		IList<JobDeclaration<object, ?>> declarations = jobDeclarations[procDef.Key];
		if (declarations != null)
		{
//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: Iterator<JobDeclaration<?, ?>> iter = declarations.iterator();
		  IEnumerator<JobDeclaration<object, ?>> iter = declarations.GetEnumerator();
		  while (iter.MoveNext())
		  {
//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: JobDeclaration<?, ?> jobDcl = iter.Current;
			JobDeclaration<object, ?> jobDcl = iter.Current;
			if (jobDcl.JobConfiguration.Equals(jobConfiguration, StringComparison.OrdinalIgnoreCase) && jobDcl.ActivityId.Equals(activity.ActivityId, StringComparison.OrdinalIgnoreCase))
			{
//JAVA TO C# CONVERTER TODO TASK: .NET enumerators are read-only:
			  iter.remove();
			}
		  }
		}
	  }

	  public virtual string parseDocumentation(Element element)
	  {
		IList<Element> docElements = element.elements("documentation");
		IList<string> docStrings = new List<string>();
		foreach (Element e in docElements)
		{
		  docStrings.Add(e.Text);
		}

		return parseDocumentation(docStrings);
	  }

	  public static string parseDocumentation(IList<string> docStrings)
	  {
		if (docStrings.Count == 0)
		{
		  return null;
		}

		StringBuilder builder = new StringBuilder();
		foreach (string e in docStrings)
		{
		  if (builder.Length != 0)
		  {
			builder.Append("\n\n");
		  }

		  builder.Append(e.Trim());
		}

		return builder.ToString();
	  }

	  protected internal virtual bool isCompensationHandler(Element activityElement)
	  {
		string isForCompensation = activityElement.attribute("isForCompensation");
		return !string.ReferenceEquals(isForCompensation, null) && isForCompensation.Equals(TRUE, StringComparison.OrdinalIgnoreCase);
	  }

	  /// <summary>
	  /// Parses an exclusive gateway declaration.
	  /// </summary>
	  public virtual ActivityImpl parseExclusiveGateway(Element exclusiveGwElement, ScopeImpl scope)
	  {
		ActivityImpl activity = createActivityOnScope(exclusiveGwElement, scope);
		activity.ActivityBehavior = new ExclusiveGatewayActivityBehavior();

		parseAsynchronousContinuationForActivity(exclusiveGwElement, activity);

		parseExecutionListenersOnScope(exclusiveGwElement, activity);

		foreach (BpmnParseListener parseListener in parseListeners)
		{
		  parseListener.parseExclusiveGateway(exclusiveGwElement, scope, activity);
		}
		return activity;
	  }

	  /// <summary>
	  /// Parses an inclusive gateway declaration.
	  /// </summary>
	  public virtual ActivityImpl parseInclusiveGateway(Element inclusiveGwElement, ScopeImpl scope)
	  {
		ActivityImpl activity = createActivityOnScope(inclusiveGwElement, scope);
		activity.ActivityBehavior = new InclusiveGatewayActivityBehavior();

		parseAsynchronousContinuationForActivity(inclusiveGwElement, activity);

		parseExecutionListenersOnScope(inclusiveGwElement, activity);

		foreach (BpmnParseListener parseListener in parseListeners)
		{
		  parseListener.parseInclusiveGateway(inclusiveGwElement, scope, activity);
		}
		return activity;
	  }

	  public virtual ActivityImpl parseEventBasedGateway(Element eventBasedGwElement, Element parentElement, ScopeImpl scope)
	  {
		ActivityImpl activity = createActivityOnScope(eventBasedGwElement, scope);
		activity.ActivityBehavior = new EventBasedGatewayActivityBehavior();
		activity.Scope = true;

		parseAsynchronousContinuationForActivity(eventBasedGwElement, activity);

		if (activity.AsyncAfter)
		{
		  addError("'asyncAfter' not supported for " + eventBasedGwElement.TagName + " elements.", eventBasedGwElement);
		}

		parseExecutionListenersOnScope(eventBasedGwElement, activity);

		foreach (BpmnParseListener parseListener in parseListeners)
		{
		  parseListener.parseEventBasedGateway(eventBasedGwElement, scope, activity);
		}

		// find all outgoing sequence flows:
		IList<Element> sequenceFlows = parentElement.elements("sequenceFlow");

		// collect all siblings in a map
		IDictionary<string, Element> siblingsMap = new Dictionary<string, Element>();
		IList<Element> siblings = parentElement.elements();
		foreach (Element sibling in siblings)
		{
		  siblingsMap[sibling.attribute("id")] = sibling;
		}

		foreach (Element sequenceFlow in sequenceFlows)
		{

		  string sourceRef = sequenceFlow.attribute("sourceRef");
		  string targetRef = sequenceFlow.attribute("targetRef");

		  if (activity.Id.Equals(sourceRef))
		  {
			Element sibling = siblingsMap[targetRef];
			if (sibling != null)
			{
			  if (sibling.TagName.Equals(ActivityTypes.INTERMEDIATE_EVENT_CATCH))
			  {
				ActivityImpl catchEventActivity = parseIntermediateCatchEvent(sibling, scope, activity);

				if (catchEventActivity != null)
				{
				  parseActivityInputOutput(sibling, catchEventActivity);
				}

			  }
			  else
			  {
				addError("Event based gateway can only be connected to elements of type intermediateCatchEvent", sibling);
			  }
			}
		  }
		}

		return activity;
	  }

	  /// <summary>
	  /// Parses a parallel gateway declaration.
	  /// </summary>
	  public virtual ActivityImpl parseParallelGateway(Element parallelGwElement, ScopeImpl scope)
	  {
		ActivityImpl activity = createActivityOnScope(parallelGwElement, scope);
		activity.ActivityBehavior = new ParallelGatewayActivityBehavior();

		parseAsynchronousContinuationForActivity(parallelGwElement, activity);

		parseExecutionListenersOnScope(parallelGwElement, activity);

		foreach (BpmnParseListener parseListener in parseListeners)
		{
		  parseListener.parseParallelGateway(parallelGwElement, scope, activity);
		}
		return activity;
	  }

	  /// <summary>
	  /// Parses a scriptTask declaration.
	  /// </summary>
	  public virtual ActivityImpl parseScriptTask(Element scriptTaskElement, ScopeImpl scope)
	  {
		ActivityImpl activity = createActivityOnScope(scriptTaskElement, scope);

		ScriptTaskActivityBehavior activityBehavior = parseScriptTaskElement(scriptTaskElement);

		if (activityBehavior != null)
		{
		  parseAsynchronousContinuationForActivity(scriptTaskElement, activity);

		  activity.ActivityBehavior = activityBehavior;

		  parseExecutionListenersOnScope(scriptTaskElement, activity);

		  foreach (BpmnParseListener parseListener in parseListeners)
		  {
			parseListener.parseScriptTask(scriptTaskElement, scope, activity);
		  }
		}

		return activity;
	  }

	  /// <summary>
	  /// Returns a <seealso cref="ScriptTaskActivityBehavior"/> for the script task element
	  /// corresponding to the script source or resource specified.
	  /// </summary>
	  /// <param name="scriptTaskElement">
	  ///          the script task element </param>
	  /// <returns> the corresponding <seealso cref="ScriptTaskActivityBehavior"/> </returns>
	  protected internal virtual ScriptTaskActivityBehavior parseScriptTaskElement(Element scriptTaskElement)
	  {
		// determine script language
		string language = scriptTaskElement.attribute("scriptFormat");
		if (string.ReferenceEquals(language, null))
		{
		  language = ScriptingEngines.DEFAULT_SCRIPTING_LANGUAGE;
		}
		string resultVariableName = parseResultVariable(scriptTaskElement);

		// determine script source
		string scriptSource = null;
		Element scriptElement = scriptTaskElement.element("script");
		if (scriptElement != null)
		{
		  scriptSource = scriptElement.Text;
		}
		string scriptResource = scriptTaskElement.attributeNS(CAMUNDA_BPMN_EXTENSIONS_NS, PROPERTYNAME_RESOURCE);

		try
		{
		  ExecutableScript script = ScriptUtil.getScript(language, scriptSource, scriptResource, expressionManager);
		  return new ScriptTaskActivityBehavior(script, resultVariableName);
		}
		catch (ProcessEngineException e)
		{
		  addError("Unable to process ScriptTask: " + e.Message, scriptElement);
		  return null;
		}
	  }

	  protected internal virtual string parseResultVariable(Element element)
	  {
		// determine if result variable exists
		string resultVariableName = element.attributeNS(CAMUNDA_BPMN_EXTENSIONS_NS, "resultVariable");
		if (string.ReferenceEquals(resultVariableName, null))
		{
		  // for backwards compatible reasons
		  resultVariableName = element.attributeNS(CAMUNDA_BPMN_EXTENSIONS_NS, "resultVariableName");
		}
		return resultVariableName;
	  }

	  /// <summary>
	  /// Parses a serviceTask declaration.
	  /// </summary>
	  public virtual ActivityImpl parseServiceTask(Element serviceTaskElement, ScopeImpl scope)
	  {
		return parseServiceTaskLike("serviceTask", serviceTaskElement, scope);
	  }

	  public virtual ActivityImpl parseServiceTaskLike(string elementName, Element serviceTaskElement, ScopeImpl scope)
	  {
		ActivityImpl activity = createActivityOnScope(serviceTaskElement, scope);

		string type = serviceTaskElement.attributeNS(CAMUNDA_BPMN_EXTENSIONS_NS, TYPE);
		string className = serviceTaskElement.attributeNS(CAMUNDA_BPMN_EXTENSIONS_NS, PROPERTYNAME_CLASS);
		string expression = serviceTaskElement.attributeNS(CAMUNDA_BPMN_EXTENSIONS_NS, PROPERTYNAME_EXPRESSION);
		string delegateExpression = serviceTaskElement.attributeNS(CAMUNDA_BPMN_EXTENSIONS_NS, PROPERTYNAME_DELEGATE_EXPRESSION);
		string resultVariableName = parseResultVariable(serviceTaskElement);

		parseAsynchronousContinuationForActivity(serviceTaskElement, activity);

		if (!string.ReferenceEquals(type, null))
		{
		  if (type.Equals("mail", StringComparison.OrdinalIgnoreCase))
		  {
			parseEmailServiceTask(activity, serviceTaskElement, parseFieldDeclarations(serviceTaskElement));
		  }
		  else if (type.Equals("shell", StringComparison.OrdinalIgnoreCase))
		  {
			parseShellServiceTask(activity, serviceTaskElement, parseFieldDeclarations(serviceTaskElement));
		  }
		  else if (type.Equals("external", StringComparison.OrdinalIgnoreCase))
		  {
			parseExternalServiceTask(activity, serviceTaskElement);
		  }
		  else
		  {
			addError("Invalid usage of type attribute on " + elementName + ": '" + type + "'", serviceTaskElement);
		  }
		}
		else if (!string.ReferenceEquals(className, null) && className.Trim().Length > 0)
		{
		  if (!string.ReferenceEquals(resultVariableName, null))
		  {
			addError("'resultVariableName' not supported for " + elementName + " elements using 'class'", serviceTaskElement);
		  }
		  activity.ActivityBehavior = new ClassDelegateActivityBehavior(className, parseFieldDeclarations(serviceTaskElement));

		}
		else if (!string.ReferenceEquals(delegateExpression, null))
		{
		  if (!string.ReferenceEquals(resultVariableName, null))
		  {
			addError("'resultVariableName' not supported for " + elementName + " elements using 'delegateExpression'", serviceTaskElement);
		  }
		  activity.ActivityBehavior = new ServiceTaskDelegateExpressionActivityBehavior(expressionManager.createExpression(delegateExpression), parseFieldDeclarations(serviceTaskElement));

		}
		else if (!string.ReferenceEquals(expression, null) && expression.Trim().Length > 0)
		{
		  activity.ActivityBehavior = new ServiceTaskExpressionActivityBehavior(expressionManager.createExpression(expression), resultVariableName);

		}

		parseExecutionListenersOnScope(serviceTaskElement, activity);

		foreach (BpmnParseListener parseListener in parseListeners)
		{
		  parseListener.parseServiceTask(serviceTaskElement, scope, activity);
		}

		// activity behavior could be set by a listener (e.g. connector); thus,
		// check is after listener invocation
		if (activity.ActivityBehavior == null)
		{
		  addError("One of the attributes 'class', 'delegateExpression', 'type', or 'expression' is mandatory on " + elementName + ".", serviceTaskElement);
		}

		return activity;
	  }

	  /// <summary>
	  /// Parses a businessRuleTask declaration.
	  /// </summary>
	  public virtual ActivityImpl parseBusinessRuleTask(Element businessRuleTaskElement, ScopeImpl scope)
	  {
		string decisionRef = businessRuleTaskElement.attributeNS(CAMUNDA_BPMN_EXTENSIONS_NS, "decisionRef");
		if (!string.ReferenceEquals(decisionRef, null))
		{
		  return parseDmnBusinessRuleTask(businessRuleTaskElement, scope);
		}
		else
		{
		  return parseServiceTaskLike("businessRuleTask", businessRuleTaskElement, scope);
		}
	  }

	  /// <summary>
	  /// Parse a Business Rule Task which references a decision.
	  /// </summary>
	  protected internal virtual ActivityImpl parseDmnBusinessRuleTask(Element businessRuleTaskElement, ScopeImpl scope)
	  {
		ActivityImpl activity = createActivityOnScope(businessRuleTaskElement, scope);
		// the activity is a scope since the result variable is stored as local variable
		activity.Scope = true;

		parseAsynchronousContinuationForActivity(businessRuleTaskElement, activity);

		string decisionRef = businessRuleTaskElement.attributeNS(CAMUNDA_BPMN_EXTENSIONS_NS, "decisionRef");

		BaseCallableElement callableElement = new BaseCallableElement();
		callableElement.DeploymentId = deployment_Renamed.Id;

		ParameterValueProvider definitionKeyProvider = createParameterValueProvider(decisionRef, expressionManager);
		callableElement.DefinitionKeyValueProvider = definitionKeyProvider;

		parseBinding(businessRuleTaskElement, activity, callableElement, "decisionRefBinding");
		parseVersion(businessRuleTaskElement, activity, callableElement, "decisionRefBinding", "decisionRefVersion");
		parseVersionTag(businessRuleTaskElement, activity, callableElement, "decisionRefBinding", "decisionRefVersionTag");
		parseTenantId(businessRuleTaskElement, activity, callableElement, "decisionRefTenantId");

		string resultVariable = parseResultVariable(businessRuleTaskElement);
		DecisionResultMapper decisionResultMapper = parseDecisionResultMapper(businessRuleTaskElement);

		DmnBusinessRuleTaskActivityBehavior behavior = new DmnBusinessRuleTaskActivityBehavior(callableElement, resultVariable, decisionResultMapper);
		activity.ActivityBehavior = behavior;

		parseExecutionListenersOnScope(businessRuleTaskElement, activity);

		foreach (BpmnParseListener parseListener in parseListeners)
		{
		  parseListener.parseBusinessRuleTask(businessRuleTaskElement, scope, activity);
		}

		return activity;
	  }

	  protected internal virtual DecisionResultMapper parseDecisionResultMapper(Element businessRuleTaskElement)
	  {
		// default mapper is 'resultList'
		string decisionResultMapper = businessRuleTaskElement.attributeNS(CAMUNDA_BPMN_EXTENSIONS_NS, "mapDecisionResult");
		DecisionResultMapper mapper = DecisionEvaluationUtil.getDecisionResultMapperForName(decisionResultMapper);

		if (mapper == null)
		{
		  addError("No decision result mapper found for name '" + decisionResultMapper + "'. Supported mappers are 'singleEntry', 'singleResult', 'collectEntries' and 'resultList'.", businessRuleTaskElement);
		}

		return mapper;
	  }

	  /// <summary>
	  /// Parse async continuation of an activity and create async jobs for the activity.
	  /// <br/> <br/>
	  /// When the activity is marked as multi instance, then async jobs create instead for the multi instance body.
	  /// When the wrapped activity has async characteristics in 'multiInstanceLoopCharacteristics' element,
	  /// then async jobs create additionally for the wrapped activity.
	  /// </summary>
	  protected internal virtual void parseAsynchronousContinuationForActivity(Element activityElement, ActivityImpl activity)
	  {
		// can't use #getMultiInstanceScope here to determine whether the task is multi-instance,
		// since the property hasn't been set yet (cf parseActivity)
		ActivityImpl parentFlowScopeActivity = activity.ParentFlowScopeActivity;
		if (parentFlowScopeActivity != null && parentFlowScopeActivity.ActivityBehavior is MultiInstanceActivityBehavior && !activity.CompensationHandler)
		{

		  parseAsynchronousContinuation(activityElement, parentFlowScopeActivity);

		  Element miLoopCharacteristics = activityElement.element("multiInstanceLoopCharacteristics");
		  parseAsynchronousContinuation(miLoopCharacteristics, activity);

		}
		else
		{
		  parseAsynchronousContinuation(activityElement, activity);
		}
	  }

	  /// <summary>
	  /// Parse async continuation of the given element and create async jobs for the activity.
	  /// </summary>
	  /// <param name="element"> with async characteristics </param>
	  /// <param name="activity"> </param>
	  protected internal virtual void parseAsynchronousContinuation(Element element, ActivityImpl activity)
	  {

		bool isAsyncBefore = isAsyncBefore(element);
		bool isAsyncAfter = isAsyncAfter(element);
		bool exclusive = isExclusive(element);

		// set properties on activity
		activity.setAsyncBefore(isAsyncBefore, exclusive);
		activity.setAsyncAfter(isAsyncAfter, exclusive);
	  }

	  protected internal virtual ParameterValueProvider parsePriority(Element element, string priorityAttribute)
	  {
		string priorityAttributeValue = element.attributeNS(CAMUNDA_BPMN_EXTENSIONS_NS, priorityAttribute);

		if (string.ReferenceEquals(priorityAttributeValue, null))
		{
		  return null;

		}
		else
		{
		  object value = priorityAttributeValue;
		  if (!StringUtil.isExpression(priorityAttributeValue))
		  {
			// constant values must be valid integers
			try
			{
			  value = int.Parse(priorityAttributeValue);

			}
			catch (System.FormatException)
			{
			  addError("Value '" + priorityAttributeValue + "' for attribute '" + priorityAttribute + "' is not a valid number", element);
			}
		  }

		  return createParameterValueProvider(value, expressionManager);
		}
	  }

	  protected internal virtual ParameterValueProvider parseTopic(Element element, string topicAttribute)
	  {
		string topicAttributeValue = element.attributeNS(CAMUNDA_BPMN_EXTENSIONS_NS, topicAttribute);

		if (string.ReferenceEquals(topicAttributeValue, null))
		{
		  addError("External tasks must specify a 'topic' attribute in the camunda namespace", element);
		  return null;

		}
		else
		{
		  return createParameterValueProvider(topicAttributeValue, expressionManager);
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") protected void addMessageJobDeclarationToActivity(MessageJobDeclaration messageJobDeclaration, ActivityImpl activity)
	  protected internal virtual void addMessageJobDeclarationToActivity(MessageJobDeclaration messageJobDeclaration, ActivityImpl activity)
	  {
		IList<MessageJobDeclaration> messageJobDeclarations = (IList<MessageJobDeclaration>) activity.getProperty(PROPERTYNAME_MESSAGE_JOB_DECLARATION);
		if (messageJobDeclarations == null)
		{
		  messageJobDeclarations = new List<MessageJobDeclaration>();
		  activity.setProperty(PROPERTYNAME_MESSAGE_JOB_DECLARATION, messageJobDeclarations);
		}
		messageJobDeclarations.Add(messageJobDeclaration);
	  }

	  protected internal virtual void addJobDeclarationToProcessDefinition<T1>(JobDeclaration<T1> jobDeclaration, ProcessDefinition processDefinition)
	  {
		string key = processDefinition.Key;

//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: List<JobDeclaration<?, ?>> containingJobDeclarations = jobDeclarations.get(key);
		IList<JobDeclaration<object, ?>> containingJobDeclarations = jobDeclarations[key];
		if (containingJobDeclarations == null)
		{
//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: containingJobDeclarations = new ArrayList<JobDeclaration<?, ?>>();
		  containingJobDeclarations = new List<JobDeclaration<object, ?>>();
		  jobDeclarations[key] = containingJobDeclarations;
		}

		containingJobDeclarations.Add(jobDeclaration);
	  }

	  /// <summary>
	  /// Parses a sendTask declaration.
	  /// </summary>
	  public virtual ActivityImpl parseSendTask(Element sendTaskElement, ScopeImpl scope)
	  {
		if (isServiceTaskLike(sendTaskElement))
		{
		  // CAM-942: If expression or class is set on a SendTask it behaves like a service task
		  // to allow implementing the send handling yourself
		  return parseServiceTaskLike("sendTask", sendTaskElement, scope);
		}
		else
		{
		  ActivityImpl activity = createActivityOnScope(sendTaskElement, scope);

		  parseAsynchronousContinuationForActivity(sendTaskElement, activity);
		  parseExecutionListenersOnScope(sendTaskElement, activity);

		  foreach (BpmnParseListener parseListener in parseListeners)
		  {
			parseListener.parseSendTask(sendTaskElement, scope, activity);
		  }

		  // activity behavior could be set by a listener; thus, check is after listener invocation
		  if (activity.ActivityBehavior == null)
		  {
			addError("One of the attributes 'class', 'delegateExpression', 'type', or 'expression' is mandatory on sendTask.", sendTaskElement);
		  }

		  return activity;
		}
	  }

	  protected internal virtual void parseEmailServiceTask(ActivityImpl activity, Element serviceTaskElement, IList<FieldDeclaration> fieldDeclarations)
	  {
		validateFieldDeclarationsForEmail(serviceTaskElement, fieldDeclarations);
		activity.ActivityBehavior = (MailActivityBehavior) instantiateDelegate(typeof(MailActivityBehavior), fieldDeclarations);
	  }

	  protected internal virtual void parseShellServiceTask(ActivityImpl activity, Element serviceTaskElement, IList<FieldDeclaration> fieldDeclarations)
	  {
		validateFieldDeclarationsForShell(serviceTaskElement, fieldDeclarations);
		activity.ActivityBehavior = (ActivityBehavior) instantiateDelegate(typeof(ShellActivityBehavior), fieldDeclarations);
	  }

	  protected internal virtual void parseExternalServiceTask(ActivityImpl activity, Element serviceTaskElement)
	  {
		activity.Scope = true;

		ParameterValueProvider topicNameProvider = parseTopic(serviceTaskElement, PROPERTYNAME_EXTERNAL_TASK_TOPIC);
		ParameterValueProvider priorityProvider = parsePriority(serviceTaskElement, PROPERTYNAME_TASK_PRIORITY);
		activity.ActivityBehavior = new ExternalTaskActivityBehavior(topicNameProvider, priorityProvider);
	  }

	  protected internal virtual void validateFieldDeclarationsForEmail(Element serviceTaskElement, IList<FieldDeclaration> fieldDeclarations)
	  {
		bool toDefined = false;
		bool textOrHtmlDefined = false;
		foreach (FieldDeclaration fieldDeclaration in fieldDeclarations)
		{
		  if (fieldDeclaration.Name.Equals("to"))
		  {
			toDefined = true;
		  }
		  if (fieldDeclaration.Name.Equals("html"))
		  {
			textOrHtmlDefined = true;
		  }
		  if (fieldDeclaration.Name.Equals("text"))
		  {
			textOrHtmlDefined = true;
		  }
		}

		if (!toDefined)
		{
		  addError("No recipient is defined on the mail activity", serviceTaskElement);
		}
		if (!textOrHtmlDefined)
		{
		  addError("Text or html field should be provided", serviceTaskElement);
		}
	  }

	  protected internal virtual void validateFieldDeclarationsForShell(Element serviceTaskElement, IList<FieldDeclaration> fieldDeclarations)
	  {
		bool shellCommandDefined = false;

		foreach (FieldDeclaration fieldDeclaration in fieldDeclarations)
		{
		  string fieldName = fieldDeclaration.Name;
		  FixedValue fieldFixedValue = (FixedValue) fieldDeclaration.Value;
		  string fieldValue = fieldFixedValue.ExpressionText;

		  shellCommandDefined |= fieldName.Equals("command");

		  if ((fieldName.Equals("wait") || fieldName.Equals("redirectError") || fieldName.Equals("cleanEnv")) && !fieldValue.ToLower().Equals(TRUE) && !fieldValue.ToLower().Equals("false"))
		  {
			addError("undefined value for shell " + fieldName + " parameter :" + fieldValue.ToString(), serviceTaskElement);
		  }

		}

		if (!shellCommandDefined)
		{
		  addError("No shell command is defined on the shell activity", serviceTaskElement);
		}
	  }

	  public virtual IList<FieldDeclaration> parseFieldDeclarations(Element element)
	  {
		IList<FieldDeclaration> fieldDeclarations = new List<FieldDeclaration>();

		Element elementWithFieldInjections = element.element("extensionElements");
		if (elementWithFieldInjections == null)
		{ // Custom extensions will just
												  // have the <field.. as a
												  // subelement
		  elementWithFieldInjections = element;
		}
		IList<Element> fieldDeclarationElements = elementWithFieldInjections.elementsNS(CAMUNDA_BPMN_EXTENSIONS_NS, "field");
		if (fieldDeclarationElements != null && fieldDeclarationElements.Count > 0)
		{

		  foreach (Element fieldDeclarationElement in fieldDeclarationElements)
		  {
			FieldDeclaration fieldDeclaration = parseFieldDeclaration(element, fieldDeclarationElement);
			if (fieldDeclaration != null)
			{
			  fieldDeclarations.Add(fieldDeclaration);
			}
		  }
		}

		return fieldDeclarations;
	  }

	  protected internal virtual FieldDeclaration parseFieldDeclaration(Element serviceTaskElement, Element fieldDeclarationElement)
	  {
		string fieldName = fieldDeclarationElement.attribute("name");

		FieldDeclaration fieldDeclaration = parseStringFieldDeclaration(fieldDeclarationElement, serviceTaskElement, fieldName);
		if (fieldDeclaration == null)
		{
		  fieldDeclaration = parseExpressionFieldDeclaration(fieldDeclarationElement, serviceTaskElement, fieldName);
		}

		if (fieldDeclaration == null)
		{
		  addError("One of the following is mandatory on a field declaration: one of attributes stringValue|expression " + "or one of child elements string|expression", serviceTaskElement);
		}
		return fieldDeclaration;
	  }

	  protected internal virtual FieldDeclaration parseStringFieldDeclaration(Element fieldDeclarationElement, Element serviceTaskElement, string fieldName)
	  {
		try
		{
		  string fieldValue = getStringValueFromAttributeOrElement("stringValue", "string", fieldDeclarationElement);
		  if (!string.ReferenceEquals(fieldValue, null))
		  {
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
			return new FieldDeclaration(fieldName, typeof(Expression).FullName, new FixedValue(fieldValue));
		  }
		}
		catch (ProcessEngineException ae)
		{
		  if (ae.Message.contains("multiple elements with tag name"))
		  {
			addError("Multiple string field declarations found", serviceTaskElement);
		  }
		  else
		  {
			addError("Error when paring field declarations: " + ae.Message, serviceTaskElement);
		  }
		}
		return null;
	  }

	  protected internal virtual FieldDeclaration parseExpressionFieldDeclaration(Element fieldDeclarationElement, Element serviceTaskElement, string fieldName)
	  {
		try
		{
		  string expression = getStringValueFromAttributeOrElement(PROPERTYNAME_EXPRESSION, PROPERTYNAME_EXPRESSION, fieldDeclarationElement);
		  if (!string.ReferenceEquals(expression, null) && expression.Trim().Length > 0)
		  {
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
			return new FieldDeclaration(fieldName, typeof(Expression).FullName, expressionManager.createExpression(expression));
		  }
		}
		catch (ProcessEngineException ae)
		{
		  if (ae.Message.contains("multiple elements with tag name"))
		  {
			addError("Multiple expression field declarations found", serviceTaskElement);
		  }
		  else
		  {
			addError("Error when paring field declarations: " + ae.Message, serviceTaskElement);
		  }
		}
		return null;
	  }

	  protected internal virtual string getStringValueFromAttributeOrElement(string attributeName, string elementName, Element element)
	  {
		string value = null;

		string attributeValue = element.attribute(attributeName);
		Element childElement = element.elementNS(CAMUNDA_BPMN_EXTENSIONS_NS, elementName);
		string stringElementText = null;

		if (!string.ReferenceEquals(attributeValue, null) && childElement != null)
		{
		  addError("Can't use attribute '" + attributeName + "' and element '" + elementName + "' together, only use one", element);
		}
		else if (childElement != null)
		{
		  stringElementText = childElement.Text;
		  if (string.ReferenceEquals(stringElementText, null) || stringElementText.Length == 0)
		  {
			addError("No valid value found in attribute '" + attributeName + "' nor element '" + elementName + "'", element);
		  }
		  else
		  {
			// Use text of element
			value = stringElementText;
		  }
		}
		else if (!string.ReferenceEquals(attributeValue, null) && attributeValue.Length > 0)
		{
		  // Using attribute
		  value = attributeValue;
		}

		return value;
	  }

	  /// <summary>
	  /// Parses a task with no specific type (behaves as passthrough).
	  /// </summary>
	  public virtual ActivityImpl parseTask(Element taskElement, ScopeImpl scope)
	  {
		ActivityImpl activity = createActivityOnScope(taskElement, scope);
		activity.ActivityBehavior = new TaskActivityBehavior();

		parseAsynchronousContinuationForActivity(taskElement, activity);

		parseExecutionListenersOnScope(taskElement, activity);

		foreach (BpmnParseListener parseListener in parseListeners)
		{
		  parseListener.parseTask(taskElement, scope, activity);
		}
	//    createMessageJobDeclForAsyncActivity(activity, true);
		return activity;
	  }

	  /// <summary>
	  /// Parses a manual task.
	  /// </summary>
	  public virtual ActivityImpl parseManualTask(Element manualTaskElement, ScopeImpl scope)
	  {
		ActivityImpl activity = createActivityOnScope(manualTaskElement, scope);
		activity.ActivityBehavior = new ManualTaskActivityBehavior();

		parseAsynchronousContinuationForActivity(manualTaskElement, activity);

		parseExecutionListenersOnScope(manualTaskElement, activity);

		foreach (BpmnParseListener parseListener in parseListeners)
		{
		  parseListener.parseManualTask(manualTaskElement, scope, activity);
		}
		return activity;
	  }

	  /// <summary>
	  /// Parses a receive task.
	  /// </summary>
	  public virtual ActivityImpl parseReceiveTask(Element receiveTaskElement, ScopeImpl scope)
	  {
		ActivityImpl activity = createActivityOnScope(receiveTaskElement, scope);
		activity.ActivityBehavior = new ReceiveTaskActivityBehavior();

		parseAsynchronousContinuationForActivity(receiveTaskElement, activity);

		parseExecutionListenersOnScope(receiveTaskElement, activity);

		if (!string.ReferenceEquals(receiveTaskElement.attribute("messageRef"), null))
		{
		  activity.Scope = true;
		  activity.EventScope = activity;
		  EventSubscriptionDeclaration declaration = parseMessageEventDefinition(receiveTaskElement);
		  declaration.ActivityId = activity.ActivityId;
		  declaration.EventScopeActivityId = activity.ActivityId;
		  addEventSubscriptionDeclaration(declaration, activity, receiveTaskElement);
		}

		foreach (BpmnParseListener parseListener in parseListeners)
		{
		  parseListener.parseReceiveTask(receiveTaskElement, scope, activity);
		}
		return activity;
	  }

	  /* userTask specific finals */

	  protected internal const string HUMAN_PERFORMER = "humanPerformer";
	  protected internal const string POTENTIAL_OWNER = "potentialOwner";

	  protected internal const string RESOURCE_ASSIGNMENT_EXPR = "resourceAssignmentExpression";
	  protected internal const string FORMAL_EXPRESSION = "formalExpression";

	  protected internal const string USER_PREFIX = "user(";
	  protected internal const string GROUP_PREFIX = "group(";

	  protected internal const string ASSIGNEE_EXTENSION = "assignee";
	  protected internal const string CANDIDATE_USERS_EXTENSION = "candidateUsers";
	  protected internal const string CANDIDATE_GROUPS_EXTENSION = "candidateGroups";
	  protected internal const string DUE_DATE_EXTENSION = "dueDate";
	  protected internal const string FOLLOW_UP_DATE_EXTENSION = "followUpDate";
	  protected internal const string PRIORITY_EXTENSION = "priority";

	  /// <summary>
	  /// Parses a userTask declaration.
	  /// </summary>
	  public virtual ActivityImpl parseUserTask(Element userTaskElement, ScopeImpl scope)
	  {
		ActivityImpl activity = createActivityOnScope(userTaskElement, scope);

		parseAsynchronousContinuationForActivity(userTaskElement, activity);

		TaskDefinition taskDefinition = parseTaskDefinition(userTaskElement, activity.Id, (ProcessDefinitionEntity) scope.ProcessDefinition);
		TaskDecorator taskDecorator = new TaskDecorator(taskDefinition, expressionManager);

		UserTaskActivityBehavior userTaskActivity = new UserTaskActivityBehavior(taskDecorator);
		activity.ActivityBehavior = userTaskActivity;

		parseProperties(userTaskElement, activity);
		parseExecutionListenersOnScope(userTaskElement, activity);

		foreach (BpmnParseListener parseListener in parseListeners)
		{
		  parseListener.parseUserTask(userTaskElement, scope, activity);
		}
		return activity;
	  }

	  public virtual TaskDefinition parseTaskDefinition(Element taskElement, string taskDefinitionKey, ProcessDefinitionEntity processDefinition)
	  {
		TaskFormHandler taskFormHandler;
		string taskFormHandlerClassName = taskElement.attributeNS(CAMUNDA_BPMN_EXTENSIONS_NS, "formHandlerClass");
		if (!string.ReferenceEquals(taskFormHandlerClassName, null))
		{
		  taskFormHandler = (TaskFormHandler) ReflectUtil.instantiate(taskFormHandlerClassName);
		}
		else
		{
		  taskFormHandler = new DefaultTaskFormHandler();
		}
		taskFormHandler.parseConfiguration(taskElement, deployment_Renamed, processDefinition, this);

		TaskDefinition taskDefinition = new TaskDefinition(new DelegateTaskFormHandler(taskFormHandler, deployment_Renamed));

		taskDefinition.Key = taskDefinitionKey;
		processDefinition.TaskDefinitions[taskDefinitionKey] = taskDefinition;

		string formKeyAttribute = taskElement.attributeNS(CAMUNDA_BPMN_EXTENSIONS_NS, "formKey");

		if (!string.ReferenceEquals(formKeyAttribute, null))
		{
		  taskDefinition.FormKey = expressionManager.createExpression(formKeyAttribute);
		}

		string name = taskElement.attribute("name");
		if (!string.ReferenceEquals(name, null))
		{
		  taskDefinition.NameExpression = expressionManager.createExpression(name);
		}

		string descriptionStr = parseDocumentation(taskElement);
		if (!string.ReferenceEquals(descriptionStr, null))
		{
		  taskDefinition.DescriptionExpression = expressionManager.createExpression(descriptionStr);
		}

		parseHumanPerformer(taskElement, taskDefinition);
		parsePotentialOwner(taskElement, taskDefinition);

		// Activiti custom extension
		parseUserTaskCustomExtensions(taskElement, taskDefinition);

		return taskDefinition;
	  }

	  protected internal virtual void parseHumanPerformer(Element taskElement, TaskDefinition taskDefinition)
	  {
		IList<Element> humanPerformerElements = taskElement.elements(HUMAN_PERFORMER);

		if (humanPerformerElements.Count > 1)
		{
		  addError("Invalid task definition: multiple " + HUMAN_PERFORMER + " sub elements defined for " + taskDefinition.NameExpression, taskElement);
		}
		else if (humanPerformerElements.Count == 1)
		{
		  Element humanPerformerElement = humanPerformerElements[0];
		  if (humanPerformerElement != null)
		  {
			parseHumanPerformerResourceAssignment(humanPerformerElement, taskDefinition);
		  }
		}
	  }

	  protected internal virtual void parsePotentialOwner(Element taskElement, TaskDefinition taskDefinition)
	  {
		IList<Element> potentialOwnerElements = taskElement.elements(POTENTIAL_OWNER);
		foreach (Element potentialOwnerElement in potentialOwnerElements)
		{
		  parsePotentialOwnerResourceAssignment(potentialOwnerElement, taskDefinition);
		}
	  }

	  protected internal virtual void parseHumanPerformerResourceAssignment(Element performerElement, TaskDefinition taskDefinition)
	  {
		Element raeElement = performerElement.element(RESOURCE_ASSIGNMENT_EXPR);
		if (raeElement != null)
		{
		  Element feElement = raeElement.element(FORMAL_EXPRESSION);
		  if (feElement != null)
		  {
			taskDefinition.AssigneeExpression = expressionManager.createExpression(feElement.Text);
		  }
		}
	  }

	  protected internal virtual void parsePotentialOwnerResourceAssignment(Element performerElement, TaskDefinition taskDefinition)
	  {
		Element raeElement = performerElement.element(RESOURCE_ASSIGNMENT_EXPR);
		if (raeElement != null)
		{
		  Element feElement = raeElement.element(FORMAL_EXPRESSION);
		  if (feElement != null)
		  {
			IList<string> assignmentExpressions = parseCommaSeparatedList(feElement.Text);
			foreach (string assignmentExpression in assignmentExpressions)
			{
			  assignmentExpression = assignmentExpression.Trim();
			  if (assignmentExpression.StartsWith(USER_PREFIX, StringComparison.Ordinal))
			  {
				string userAssignementId = getAssignmentId(assignmentExpression, USER_PREFIX);
				taskDefinition.addCandidateUserIdExpression(expressionManager.createExpression(userAssignementId));
			  }
			  else if (assignmentExpression.StartsWith(GROUP_PREFIX, StringComparison.Ordinal))
			  {
				string groupAssignementId = getAssignmentId(assignmentExpression, GROUP_PREFIX);
				taskDefinition.addCandidateGroupIdExpression(expressionManager.createExpression(groupAssignementId));
			  }
			  else
			  { // default: given string is a goupId, as-is.
				taskDefinition.addCandidateGroupIdExpression(expressionManager.createExpression(assignmentExpression));
			  }
			}
		  }
		}
	  }

	  protected internal virtual string getAssignmentId(string expression, string prefix)
	  {
		return StringHelper.SubstringSpecial(expression, prefix.Length, expression.Length - 1).Trim();
	  }

	  protected internal virtual void parseUserTaskCustomExtensions(Element taskElement, TaskDefinition taskDefinition)
	  {

		// assignee
		string assignee = taskElement.attributeNS(CAMUNDA_BPMN_EXTENSIONS_NS, ASSIGNEE_EXTENSION);
		if (!string.ReferenceEquals(assignee, null))
		{
		  if (taskDefinition.AssigneeExpression == null)
		  {
			taskDefinition.AssigneeExpression = expressionManager.createExpression(assignee);
		  }
		  else
		  {
			addError("Invalid usage: duplicate assignee declaration for task " + taskDefinition.NameExpression, taskElement);
		  }
		}

		// Candidate users
		string candidateUsersString = taskElement.attributeNS(CAMUNDA_BPMN_EXTENSIONS_NS, CANDIDATE_USERS_EXTENSION);
		if (!string.ReferenceEquals(candidateUsersString, null))
		{
		  IList<string> candidateUsers = parseCommaSeparatedList(candidateUsersString);
		  foreach (string candidateUser in candidateUsers)
		  {
			taskDefinition.addCandidateUserIdExpression(expressionManager.createExpression(candidateUser.Trim()));
		  }
		}

		// Candidate groups
		string candidateGroupsString = taskElement.attributeNS(CAMUNDA_BPMN_EXTENSIONS_NS, CANDIDATE_GROUPS_EXTENSION);
		if (!string.ReferenceEquals(candidateGroupsString, null))
		{
		  IList<string> candidateGroups = parseCommaSeparatedList(candidateGroupsString);
		  foreach (string candidateGroup in candidateGroups)
		  {
			taskDefinition.addCandidateGroupIdExpression(expressionManager.createExpression(candidateGroup.Trim()));
		  }
		}

		// Task listeners
		parseTaskListeners(taskElement, taskDefinition);

		// Due date
		string dueDateExpression = taskElement.attributeNS(CAMUNDA_BPMN_EXTENSIONS_NS, DUE_DATE_EXTENSION);
		if (!string.ReferenceEquals(dueDateExpression, null))
		{
		  taskDefinition.DueDateExpression = expressionManager.createExpression(dueDateExpression);
		}

		// follow up date
		string followUpDateExpression = taskElement.attributeNS(CAMUNDA_BPMN_EXTENSIONS_NS, FOLLOW_UP_DATE_EXTENSION);
		if (!string.ReferenceEquals(followUpDateExpression, null))
		{
		  taskDefinition.FollowUpDateExpression = expressionManager.createExpression(followUpDateExpression);
		}

		// Priority
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final String priorityExpression = taskElement.attributeNS(CAMUNDA_BPMN_EXTENSIONS_NS, PRIORITY_EXTENSION);
		string priorityExpression = taskElement.attributeNS(CAMUNDA_BPMN_EXTENSIONS_NS, PRIORITY_EXTENSION);
		if (!string.ReferenceEquals(priorityExpression, null))
		{
		  taskDefinition.PriorityExpression = expressionManager.createExpression(priorityExpression);
		}
	  }

	  /// <summary>
	  /// Parses the given String as a list of comma separated entries, where an
	  /// entry can possibly be an expression that has comma's.
	  /// 
	  /// If somebody is smart enough to write a regex for this, please let us know.
	  /// </summary>
	  /// <returns> the entries of the comma separated list, trimmed. </returns>
	  protected internal virtual IList<string> parseCommaSeparatedList(string s)
	  {
		IList<string> result = new List<string>();
		if (!string.ReferenceEquals(s, null) && !"".Equals(s))
		{

		  StringCharacterIterator iterator = new StringCharacterIterator(s);
		  char c = iterator.first();

		  StringBuilder strb = new StringBuilder();
		  bool insideExpression = false;

		  while (c != StringCharacterIterator.DONE)
		  {
			if (c == '{' || c == '$')
			{
			  insideExpression = true;
			}
			else if (c == '}')
			{
			  insideExpression = false;
			}
			else if (c == ',' && !insideExpression)
			{
			  result.Add(strb.ToString().Trim());
			  strb.Remove(0, strb.Length);
			}

			if (c != ',' || (insideExpression))
			{
			  strb.Append(c);
			}

			c = iterator.next();
		  }

		  if (strb.Length > 0)
		  {
			result.Add(strb.ToString().Trim());
		  }

		}
		return result;
	  }

	  protected internal virtual void parseTaskListeners(Element userTaskElement, TaskDefinition taskDefinition)
	  {
		Element extentionsElement = userTaskElement.element("extensionElements");
		if (extentionsElement != null)
		{
		  IList<Element> taskListenerElements = extentionsElement.elementsNS(CAMUNDA_BPMN_EXTENSIONS_NS, "taskListener");
		  foreach (Element taskListenerElement in taskListenerElements)
		  {
			string eventName = taskListenerElement.attribute("event");
			if (!string.ReferenceEquals(eventName, null))
			{
			  if (org.camunda.bpm.engine.@delegate.TaskListener_Fields.EVENTNAME_CREATE.Equals(eventName) || org.camunda.bpm.engine.@delegate.TaskListener_Fields.EVENTNAME_ASSIGNMENT.Equals(eventName) || org.camunda.bpm.engine.@delegate.TaskListener_Fields.EVENTNAME_COMPLETE.Equals(eventName) || org.camunda.bpm.engine.@delegate.TaskListener_Fields.EVENTNAME_DELETE.Equals(eventName))
			  {
				TaskListener taskListener = parseTaskListener(taskListenerElement);
				taskDefinition.addTaskListener(eventName, taskListener);
			  }
			  else
			  {
				addError("Attribute 'event' must be one of {create|assignment|complete|delete}", userTaskElement);
			  }
			}
			else
			{
			  addError("Attribute 'event' is mandatory on taskListener", userTaskElement);
			}
		  }
		}
	  }

	  protected internal virtual TaskListener parseTaskListener(Element taskListenerElement)
	  {
		TaskListener taskListener = null;

		string className = taskListenerElement.attribute(PROPERTYNAME_CLASS);
		string expression = taskListenerElement.attribute(PROPERTYNAME_EXPRESSION);
		string delegateExpression = taskListenerElement.attribute(PROPERTYNAME_DELEGATE_EXPRESSION);
		Element scriptElement = taskListenerElement.elementNS(CAMUNDA_BPMN_EXTENSIONS_NS, "script");

		if (!string.ReferenceEquals(className, null))
		{
		  taskListener = new ClassDelegateTaskListener(className, parseFieldDeclarations(taskListenerElement));
		}
		else if (!string.ReferenceEquals(expression, null))
		{
		  taskListener = new ExpressionTaskListener(expressionManager.createExpression(expression));
		}
		else if (!string.ReferenceEquals(delegateExpression, null))
		{
		  taskListener = new DelegateExpressionTaskListener(expressionManager.createExpression(delegateExpression), parseFieldDeclarations(taskListenerElement));
		}
		else if (scriptElement != null)
		{
		  try
		  {
			ExecutableScript executableScript = parseCamundaScript(scriptElement);
			if (executableScript != null)
			{
			  taskListener = new ScriptTaskListener(executableScript);
			}
		  }
		  catch (BpmnParseException e)
		  {
			addError(e);
		  }
		}
		else
		{
		  addError("Element 'class', 'expression', 'delegateExpression' or 'script' is mandatory on taskListener", taskListenerElement);
		}
		return taskListener;
	  }

	  /// <summary>
	  /// Parses the end events of a certain level in the process (process,
	  /// subprocess or another scope).
	  /// </summary>
	  /// <param name="parentElement">
	  ///          The 'parent' element that contains the end events (process,
	  ///          subprocess). </param>
	  /// <param name="scope">
	  ///          The <seealso cref="ScopeImpl"/> to which the end events must be added. </param>
	  public virtual void parseEndEvents(Element parentElement, ScopeImpl scope)
	  {
		foreach (Element endEventElement in parentElement.elements("endEvent"))
		{
		  ActivityImpl activity = createActivityOnScope(endEventElement, scope);

		  Element errorEventDefinition = endEventElement.element(ERROR_EVENT_DEFINITION);
		  Element cancelEventDefinition = endEventElement.element(CANCEL_EVENT_DEFINITION);
		  Element terminateEventDefinition = endEventElement.element("terminateEventDefinition");
		  Element messageEventDefinitionElement = endEventElement.element(MESSAGE_EVENT_DEFINITION);
		  Element signalEventDefinition = endEventElement.element(SIGNAL_EVENT_DEFINITION);
		  Element compensateEventDefinitionElement = endEventElement.element(COMPENSATE_EVENT_DEFINITION);
		  Element escalationEventDefinition = endEventElement.element(ESCALATION_EVENT_DEFINITION);

		  if (errorEventDefinition != null)
		  { // error end event
			string errorRef = errorEventDefinition.attribute("errorRef");

			if (string.ReferenceEquals(errorRef, null) || "".Equals(errorRef))
			{
			  addError("'errorRef' attribute is mandatory on error end event", errorEventDefinition);
			}
			else
			{
			  Error error = errors[errorRef];
			  if (error != null && (string.ReferenceEquals(error.ErrorCode, null) || "".Equals(error.ErrorCode)))
			  {
				addError("'errorCode' is mandatory on errors referenced by throwing error event definitions, but the error '" + error.Id + "' does not define one.", errorEventDefinition);
			  }
			  activity.Properties.set(BpmnProperties.TYPE, ActivityTypes.END_EVENT_ERROR);
			  if (error != null)
			  {
				activity.ActivityBehavior = new ErrorEndEventActivityBehavior(error.ErrorCode);
			  }
			  else
			  {
				activity.ActivityBehavior = new ErrorEndEventActivityBehavior(errorRef);
			  }
			}
		  }
		  else if (cancelEventDefinition != null)
		  {
			if (scope.getProperty(BpmnProperties.TYPE.Name) == null || !scope.getProperty(BpmnProperties.TYPE.Name).Equals("transaction"))
			{
			  addError("end event with cancelEventDefinition only supported inside transaction subprocess", cancelEventDefinition);
			}
			else
			{
			  activity.Properties.set(BpmnProperties.TYPE, ActivityTypes.END_EVENT_CANCEL);
			  activity.ActivityBehavior = new CancelEndEventActivityBehavior();
			  activity.ActivityStartBehavior = ActivityStartBehavior.INTERRUPT_FLOW_SCOPE;
			  activity.setProperty(PROPERTYNAME_THROWS_COMPENSATION, true);
			  activity.Scope = true;
			}
		  }
		  else if (terminateEventDefinition != null)
		  {
			activity.Properties.set(BpmnProperties.TYPE, ActivityTypes.END_EVENT_TERMINATE);
			activity.ActivityBehavior = new TerminateEndEventActivityBehavior();
			activity.ActivityStartBehavior = ActivityStartBehavior.INTERRUPT_FLOW_SCOPE;
		  }
		  else if (messageEventDefinitionElement != null)
		  {
			if (isServiceTaskLike(messageEventDefinitionElement))
			{

			  // CAM-436 same behaviour as service task
			  ActivityImpl act = parseServiceTaskLike(ActivityTypes.END_EVENT_MESSAGE, messageEventDefinitionElement, scope);
			  activity.Properties.set(BpmnProperties.TYPE, ActivityTypes.END_EVENT_MESSAGE);
			  activity.ActivityBehavior = act.ActivityBehavior;
			  scope.Activities.Remove(act);
			}
			else
			{
			  // default to non behavior if no service task
			  // properties have been specified
			  activity.ActivityBehavior = new IntermediateThrowNoneEventActivityBehavior();
			}
		  }
		  else if (signalEventDefinition != null)
		  {
			activity.Properties.set(BpmnProperties.TYPE, ActivityTypes.END_EVENT_SIGNAL);
			EventSubscriptionDeclaration signalDefinition = parseSignalEventDefinition(signalEventDefinition, true);
			activity.ActivityBehavior = new ThrowSignalEventActivityBehavior(signalDefinition);

		  }
		  else if (compensateEventDefinitionElement != null)
		  {
			activity.Properties.set(BpmnProperties.TYPE, ActivityTypes.END_EVENT_COMPENSATION);
			CompensateEventDefinition compensateEventDefinition = parseThrowCompensateEventDefinition(compensateEventDefinitionElement, scope);
			activity.ActivityBehavior = new CompensationEventActivityBehavior(compensateEventDefinition);
			activity.setProperty(PROPERTYNAME_THROWS_COMPENSATION, true);
			activity.Scope = true;

		  }
		  else if (escalationEventDefinition != null)
		  {
			activity.Properties.set(BpmnProperties.TYPE, ActivityTypes.END_EVENT_ESCALATION);

			Escalation escalation = findEscalationForEscalationEventDefinition(escalationEventDefinition);
			if (escalation != null && string.ReferenceEquals(escalation.EscalationCode, null))
			{
			  addError("escalation end event must have an 'escalationCode'", escalationEventDefinition);
			}
			activity.ActivityBehavior = new ThrowEscalationEventActivityBehavior(escalation);

		  }
		  else
		  { // default: none end event
			activity.Properties.set(BpmnProperties.TYPE, ActivityTypes.END_EVENT_NONE);
			activity.ActivityBehavior = new NoneEndEventActivityBehavior();
		  }

		  if (activity != null)
		  {
			parseActivityInputOutput(endEventElement, activity);
		  }

		  parseAsynchronousContinuationForActivity(endEventElement, activity);

		  parseExecutionListenersOnScope(endEventElement, activity);

		  foreach (BpmnParseListener parseListener in parseListeners)
		  {
			parseListener.parseEndEvent(endEventElement, scope, activity);
		  }

		}
	  }

	  /// <summary>
	  /// Parses the boundary events of a certain 'level' (process, subprocess or
	  /// other scope).
	  /// 
	  /// Note that the boundary events are not parsed during the parsing of the bpmn
	  /// activities, since the semantics are different (boundaryEvent needs to be
	  /// added as nested activity to the reference activity on PVM level).
	  /// </summary>
	  /// <param name="parentElement">
	  ///          The 'parent' element that contains the activities (process,
	  ///          subprocess). </param>
	  /// <param name="flowScope">
	  ///          The <seealso cref="ScopeImpl"/> to which the activities must be added. </param>
	  public virtual void parseBoundaryEvents(Element parentElement, ScopeImpl flowScope)
	  {
		foreach (Element boundaryEventElement in parentElement.elements("boundaryEvent"))
		{

		  // The boundary event is attached to an activity, reference by the
		  // 'attachedToRef' attribute
		  string attachedToRef = boundaryEventElement.attribute("attachedToRef");
		  if (string.ReferenceEquals(attachedToRef, null) || attachedToRef.Equals(""))
		  {
			addError("AttachedToRef is required when using a timerEventDefinition", boundaryEventElement);
		  }

		  // Representation structure-wise is a nested activity in the activity to
		  // which its attached
		  string id = boundaryEventElement.attribute("id");

		  LOG.parsingElement("boundary event", id);

		  // Depending on the sub-element definition, the correct activityBehavior
		  // parsing is selected
		  Element timerEventDefinition = boundaryEventElement.element(TIMER_EVENT_DEFINITION);
		  Element errorEventDefinition = boundaryEventElement.element(ERROR_EVENT_DEFINITION);
		  Element signalEventDefinition = boundaryEventElement.element(SIGNAL_EVENT_DEFINITION);
		  Element cancelEventDefinition = boundaryEventElement.element(CANCEL_EVENT_DEFINITION);
		  Element compensateEventDefinition = boundaryEventElement.element(COMPENSATE_EVENT_DEFINITION);
		  Element messageEventDefinition = boundaryEventElement.element(MESSAGE_EVENT_DEFINITION);
		  Element escalationEventDefinition = boundaryEventElement.element(ESCALATION_EVENT_DEFINITION);
		  Element conditionalEventDefinition = boundaryEventElement.element(CONDITIONAL_EVENT_DEFINITION);

		  // create the boundary event activity
		  ActivityImpl boundaryEventActivity = createActivityOnScope(boundaryEventElement, flowScope);
		  parseAsynchronousContinuation(boundaryEventElement, boundaryEventActivity);

		  ActivityImpl attachedActivity = flowScope.findActivityAtLevelOfSubprocess(attachedToRef);
		  if (attachedActivity == null)
		  {
			addError("Invalid reference in boundary event. Make sure that the referenced activity is defined in the same scope as the boundary event", boundaryEventElement);
		  }

		  // determine the correct event scope (the scope in which the boundary event catches events)
		  if (compensateEventDefinition == null)
		  {
			ActivityImpl multiInstanceScope = getMultiInstanceScope(attachedActivity);
			if (multiInstanceScope != null)
			{
			  // if the boundary event is attached to a multi instance activity,
			  // then the scope of the boundary event is the multi instance body.
			  boundaryEventActivity.EventScope = multiInstanceScope;
			}
			else
			{
			  attachedActivity.Scope = true;
			  boundaryEventActivity.EventScope = attachedActivity;
			}
		  }
		  else
		  {
			boundaryEventActivity.EventScope = attachedActivity;
		  }

		  // except escalation, by default is assumed to abort the activity
		  string cancelActivityAttr = boundaryEventElement.attribute("cancelActivity", TRUE);
		  bool isCancelActivity = Convert.ToBoolean(cancelActivityAttr);

		  // determine start behavior
		  if (isCancelActivity)
		  {
			boundaryEventActivity.ActivityStartBehavior = ActivityStartBehavior.CANCEL_EVENT_SCOPE;
		  }
		  else
		  {
			boundaryEventActivity.ActivityStartBehavior = ActivityStartBehavior.CONCURRENT_IN_FLOW_SCOPE;
		  }

		  // Catch event behavior is the same for most types
		  ActivityBehavior behavior = new BoundaryEventActivityBehavior();
		  if (timerEventDefinition != null)
		  {
			parseBoundaryTimerEventDefinition(timerEventDefinition, isCancelActivity, boundaryEventActivity);

		  }
		  else if (errorEventDefinition != null)
		  {
			parseBoundaryErrorEventDefinition(errorEventDefinition, boundaryEventActivity);

		  }
		  else if (signalEventDefinition != null)
		  {
			parseBoundarySignalEventDefinition(signalEventDefinition, isCancelActivity, boundaryEventActivity);

		  }
		  else if (cancelEventDefinition != null)
		  {
			behavior = parseBoundaryCancelEventDefinition(cancelEventDefinition, boundaryEventActivity);

		  }
		  else if (compensateEventDefinition != null)
		  {
			parseBoundaryCompensateEventDefinition(compensateEventDefinition, boundaryEventActivity);

		  }
		  else if (messageEventDefinition != null)
		  {
			parseBoundaryMessageEventDefinition(messageEventDefinition, isCancelActivity, boundaryEventActivity);

		  }
		  else if (escalationEventDefinition != null)
		  {

			if (attachedActivity.SubProcessScope || attachedActivity.ActivityBehavior is CallActivityBehavior)
			{
			  parseBoundaryEscalationEventDefinition(escalationEventDefinition, isCancelActivity, boundaryEventActivity);
			}
			else
			{
			  addError("An escalation boundary event should only be attached to a subprocess or a call activity", boundaryEventElement);
			}

		  }
		  else if (conditionalEventDefinition != null)
		  {
			behavior = parseBoundaryConditionalEventDefinition(conditionalEventDefinition, isCancelActivity, boundaryEventActivity);
		  }
		  else
		  {
			addError("Unsupported boundary event type", boundaryEventElement);

		  }

		  ensureNoIoMappingDefined(boundaryEventElement);

		  boundaryEventActivity.ActivityBehavior = behavior;

		  parseExecutionListenersOnScope(boundaryEventElement, boundaryEventActivity);

		  foreach (BpmnParseListener parseListener in parseListeners)
		  {
			parseListener.parseBoundaryEvent(boundaryEventElement, flowScope, boundaryEventActivity);
		  }

		}

	  }

	  protected internal virtual ActivityImpl getMultiInstanceScope(ActivityImpl activity)
	  {
		if (activity.MultiInstance)
		{
		  return activity.ParentFlowScopeActivity;
		}
		else
		{
		  return null;
		}
	  }

	  /// <summary>
	  /// Parses a boundary timer event. The end-result will be that the given nested
	  /// activity will get the appropriate <seealso cref="ActivityBehavior"/>.
	  /// </summary>
	  /// <param name="timerEventDefinition">
	  ///          The XML element corresponding with the timer event details </param>
	  /// <param name="interrupting">
	  ///          Indicates whether this timer is interrupting. </param>
	  /// <param name="boundaryActivity">
	  ///          The activity which maps to the structure of the timer event on the
	  ///          boundary of another activity. Note that this is NOT the activity
	  ///          onto which the boundary event is attached, but a nested activity
	  ///          inside this activity, specifically created for this event. </param>
	  public virtual void parseBoundaryTimerEventDefinition(Element timerEventDefinition, bool interrupting, ActivityImpl boundaryActivity)
	  {
		boundaryActivity.Properties.set(BpmnProperties.TYPE, ActivityTypes.BOUNDARY_TIMER);
		TimerDeclarationImpl timerDeclaration = parseTimer(timerEventDefinition, boundaryActivity, TimerExecuteNestedActivityJobHandler.TYPE);

		// ACT-1427
		if (interrupting)
		{
		  timerDeclaration.InterruptingTimer = true;

		  Element timeCycleElement = timerEventDefinition.element("timeCycle");
		  if (timeCycleElement != null)
		  {
			addTimeCycleWarning(timeCycleElement, "cancelling boundary");
		  }
		}

		addTimerDeclaration(boundaryActivity.EventScope, timerDeclaration);

		foreach (BpmnParseListener parseListener in parseListeners)
		{
		  parseListener.parseBoundaryTimerEventDefinition(timerEventDefinition, interrupting, boundaryActivity);
		}
	  }

	  public virtual void parseBoundarySignalEventDefinition(Element element, bool interrupting, ActivityImpl signalActivity)
	  {
		signalActivity.Properties.set(BpmnProperties.TYPE, ActivityTypes.BOUNDARY_SIGNAL);

		EventSubscriptionDeclaration signalDefinition = parseSignalEventDefinition(element, false);
		if (string.ReferenceEquals(signalActivity.Id, null))
		{
		  addError("boundary event has no id", element);
		}
		signalDefinition.ActivityId = signalActivity.Id;
		addEventSubscriptionDeclaration(signalDefinition, signalActivity.EventScope, element);

		foreach (BpmnParseListener parseListener in parseListeners)
		{
		  parseListener.parseBoundarySignalEventDefinition(element, interrupting, signalActivity);
		}

	  }

	  public virtual void parseBoundaryMessageEventDefinition(Element element, bool interrupting, ActivityImpl messageActivity)
	  {
		messageActivity.Properties.set(BpmnProperties.TYPE, ActivityTypes.BOUNDARY_MESSAGE);

		EventSubscriptionDeclaration messageEventDefinition = parseMessageEventDefinition(element);
		if (string.ReferenceEquals(messageActivity.Id, null))
		{
		  addError("boundary event has no id", element);
		}
		messageEventDefinition.ActivityId = messageActivity.Id;
		addEventSubscriptionDeclaration(messageEventDefinition, messageActivity.EventScope, element);

		foreach (BpmnParseListener parseListener in parseListeners)
		{
		  parseListener.parseBoundaryMessageEventDefinition(element, interrupting, messageActivity);
		}

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") protected void parseTimerStartEventDefinition(org.camunda.bpm.engine.impl.util.xml.Element timerEventDefinition, ActivityImpl timerActivity, org.camunda.bpm.engine.impl.persistence.entity.ProcessDefinitionEntity processDefinition)
	  protected internal virtual void parseTimerStartEventDefinition(Element timerEventDefinition, ActivityImpl timerActivity, ProcessDefinitionEntity processDefinition)
	  {
		timerActivity.Properties.set(BpmnProperties.TYPE, ActivityTypes.START_EVENT_TIMER);
		TimerDeclarationImpl timerDeclaration = parseTimer(timerEventDefinition, timerActivity, TimerStartEventJobHandler.TYPE);
		timerDeclaration.RawJobHandlerConfiguration = processDefinition.Key;

		IList<TimerDeclarationImpl> timerDeclarations = (IList<TimerDeclarationImpl>) processDefinition.getProperty(PROPERTYNAME_START_TIMER);
		if (timerDeclarations == null)
		{
		  timerDeclarations = new List<TimerDeclarationImpl>();
		  processDefinition.setProperty(PROPERTYNAME_START_TIMER, timerDeclarations);
		}
		timerDeclarations.Add(timerDeclaration);

	  }

	  protected internal virtual void parseTimerStartEventDefinitionForEventSubprocess(Element timerEventDefinition, ActivityImpl timerActivity, bool interrupting)
	  {
		timerActivity.Properties.set(BpmnProperties.TYPE, ActivityTypes.START_EVENT_TIMER);

		TimerDeclarationImpl timerDeclaration = parseTimer(timerEventDefinition, timerActivity, TimerStartEventSubprocessJobHandler.TYPE);

		timerDeclaration.Activity = timerActivity;
		timerDeclaration.EventScopeActivityId = timerActivity.EventScope.Id;
		timerDeclaration.RawJobHandlerConfiguration = timerActivity.FlowScope.Id;
		timerDeclaration.InterruptingTimer = interrupting;

		if (interrupting)
		{
		  Element timeCycleElement = timerEventDefinition.element("timeCycle");
		  if (timeCycleElement != null)
		  {
			addTimeCycleWarning(timeCycleElement, "interrupting start");
		  }

		}

		addTimerDeclaration(timerActivity.EventScope, timerDeclaration);
	  }

	  protected internal virtual void parseEventDefinitionForSubprocess(EventSubscriptionDeclaration subscriptionDeclaration, ActivityImpl activity, Element element)
	  {
		subscriptionDeclaration.ActivityId = activity.Id;
		subscriptionDeclaration.EventScopeActivityId = activity.EventScope.Id;
		subscriptionDeclaration.StartEvent = false;
		addEventSubscriptionDeclaration(subscriptionDeclaration, activity.EventScope, element);
	  }

	  protected internal virtual void parseIntermediateSignalEventDefinition(Element element, ActivityImpl signalActivity)
	  {
		signalActivity.Properties.set(BpmnProperties.TYPE, ActivityTypes.INTERMEDIATE_EVENT_SIGNAL);

		parseSignalCatchEventDefinition(element, signalActivity, false);

		foreach (BpmnParseListener parseListener in parseListeners)
		{
		  parseListener.parseIntermediateSignalCatchEventDefinition(element, signalActivity);
		}
	  }

	  protected internal virtual void parseSignalCatchEventDefinition(Element element, ActivityImpl signalActivity, bool isStartEvent)
	  {
		EventSubscriptionDeclaration signalDefinition = parseSignalEventDefinition(element, false);
		signalDefinition.ActivityId = signalActivity.Id;
		signalDefinition.StartEvent = isStartEvent;
		addEventSubscriptionDeclaration(signalDefinition, signalActivity.EventScope, element);

		EventSubscriptionJobDeclaration catchingAsyncDeclaration = new EventSubscriptionJobDeclaration(signalDefinition);
		catchingAsyncDeclaration.JobPriorityProvider = (ParameterValueProvider) signalActivity.getProperty(PROPERTYNAME_JOB_PRIORITY);
		catchingAsyncDeclaration.Activity = signalActivity;
		signalDefinition.JobDeclaration = catchingAsyncDeclaration;
		addEventSubscriptionJobDeclaration(catchingAsyncDeclaration, signalActivity, element);
	  }

	  /// <summary>
	  /// Parses the Signal Event Definition XML including payload definition.
	  /// </summary>
	  /// <param name="signalEventDefinitionElement"> the Signal Event Definition element </param>
	  /// <param name="isThrowing"> true if a Throwing signal event is being parsed
	  /// @return </param>
	  protected internal virtual EventSubscriptionDeclaration parseSignalEventDefinition(Element signalEventDefinitionElement, bool isThrowing)
	  {
		string signalRef = signalEventDefinitionElement.attribute("signalRef");
		if (string.ReferenceEquals(signalRef, null))
		{
		  addError("signalEventDefinition does not have required property 'signalRef'", signalEventDefinitionElement);
		  return null;
		}
		else
		{
		  SignalDefinition signalDefinition = signals[resolveName(signalRef)];
		  if (signalDefinition == null)
		  {
			addError("Could not find signal with id '" + signalRef + "'", signalEventDefinitionElement);
		  }

		  EventSubscriptionDeclaration signalEventDefinition;
		  if (isThrowing)
		  {
			CallableElement payload = new CallableElement();
			parseInputParameter(signalEventDefinitionElement, payload);
			signalEventDefinition = new EventSubscriptionDeclaration(signalDefinition.Expression, EventType.SIGNAL, payload);
		  }
		  else
		  {
			signalEventDefinition = new EventSubscriptionDeclaration(signalDefinition.Expression, EventType.SIGNAL);
		  }

		  bool throwingAsync = TRUE.Equals(signalEventDefinitionElement.attributeNS(CAMUNDA_BPMN_EXTENSIONS_NS, "async", "false"));
		  signalEventDefinition.Async = throwingAsync;

		  return signalEventDefinition;
		}
	  }

	  protected internal virtual void parseIntermediateTimerEventDefinition(Element timerEventDefinition, ActivityImpl timerActivity)
	  {
		timerActivity.Properties.set(BpmnProperties.TYPE, ActivityTypes.INTERMEDIATE_EVENT_TIMER);
		TimerDeclarationImpl timerDeclaration = parseTimer(timerEventDefinition, timerActivity, TimerCatchIntermediateEventJobHandler.TYPE);

		Element timeCycleElement = timerEventDefinition.element("timeCycle");
		if (timeCycleElement != null)
		{
		  addTimeCycleWarning(timeCycleElement, "intermediate catch");
		}

		addTimerDeclaration(timerActivity.EventScope, timerDeclaration);

		foreach (BpmnParseListener parseListener in parseListeners)
		{
		  parseListener.parseIntermediateTimerEventDefinition(timerEventDefinition, timerActivity);
		}
	  }

	  protected internal virtual TimerDeclarationImpl parseTimer(Element timerEventDefinition, ActivityImpl timerActivity, string jobHandlerType)
	  {
		// TimeDate
		TimerDeclarationType type = TimerDeclarationType.DATE;
		Expression expression = parseExpression(timerEventDefinition, "timeDate");
		// TimeCycle
		if (expression == null)
		{
		  type = TimerDeclarationType.CYCLE;
		  expression = parseExpression(timerEventDefinition, "timeCycle");
		}
		// TimeDuration
		if (expression == null)
		{
		  type = TimerDeclarationType.DURATION;
		  expression = parseExpression(timerEventDefinition, "timeDuration");
		}
		// neither date, cycle or duration configured!
		if (expression == null)
		{
		  addError("Timer needs configuration (either timeDate, timeCycle or timeDuration is needed).", timerEventDefinition);
		}

		// Parse the timer declaration
		// TODO move the timer declaration into the bpmn activity or next to the TimerSession
		TimerDeclarationImpl timerDeclaration = new TimerDeclarationImpl(expression, type, jobHandlerType);
		timerDeclaration.RawJobHandlerConfiguration = timerActivity.Id;
		timerDeclaration.Exclusive = TRUE.Equals(timerEventDefinition.attributeNS(CAMUNDA_BPMN_EXTENSIONS_NS, "exclusive", JobEntity.DEFAULT_EXCLUSIVE.ToString()));
		if (string.ReferenceEquals(timerActivity.Id, null))
		{
		  addError("Attribute \"id\" is required!",timerEventDefinition);
		}
		timerDeclaration.Activity = timerActivity;
		timerDeclaration.JobConfiguration = type.ToString() + ": " + expression.ExpressionText;
		addJobDeclarationToProcessDefinition(timerDeclaration, (ProcessDefinition) timerActivity.ProcessDefinition);

		timerDeclaration.JobPriorityProvider = (ParameterValueProvider) timerActivity.getProperty(PROPERTYNAME_JOB_PRIORITY);

		return timerDeclaration;
	  }

	  protected internal virtual Expression parseExpression(Element parent, string name)
	  {
		Element value = parent.element(name);
		if (value != null)
		{
		  string expressionText = value.Text.Trim();
		  return expressionManager.createExpression(expressionText);
		}
		return null;
	  }

	  public virtual void parseBoundaryErrorEventDefinition(Element errorEventDefinition, ActivityImpl boundaryEventActivity)
	  {

		boundaryEventActivity.Properties.set(BpmnProperties.TYPE, ActivityTypes.BOUNDARY_ERROR);

		string errorRef = errorEventDefinition.attribute("errorRef");
		Error error = null;
		ErrorEventDefinition definition = new ErrorEventDefinition(boundaryEventActivity.Id);
		if (!string.ReferenceEquals(errorRef, null))
		{
		  error = errors[errorRef];
		  definition.ErrorCode = error == null ? errorRef : error.ErrorCode;
		}
		setErrorCodeVariableOnErrorEventDefinition(errorEventDefinition, definition);
		setErrorMessageVariableOnErrorEventDefinition(errorEventDefinition, definition);

		addErrorEventDefinition(definition, boundaryEventActivity.EventScope);

		foreach (BpmnParseListener parseListener in parseListeners)
		{
		  parseListener.parseBoundaryErrorEventDefinition(errorEventDefinition, true, (ActivityImpl) boundaryEventActivity.EventScope, boundaryEventActivity);
		}
	  }

	  protected internal virtual void addErrorEventDefinition(ErrorEventDefinition errorEventDefinition, ScopeImpl catchingScope)
	  {
		catchingScope.Properties.addListItem(BpmnProperties.ERROR_EVENT_DEFINITIONS, errorEventDefinition);

		IList<ErrorEventDefinition> errorEventDefinitions = catchingScope.Properties.get(BpmnProperties.ERROR_EVENT_DEFINITIONS);
		errorEventDefinitions.Sort(ErrorEventDefinition.comparator);
	  }

	  protected internal virtual void parseBoundaryEscalationEventDefinition(Element escalationEventDefinitionElement, bool cancelActivity, ActivityImpl boundaryEventActivity)
	  {
		boundaryEventActivity.Properties.set(BpmnProperties.TYPE, ActivityTypes.BOUNDARY_ESCALATION);

		EscalationEventDefinition escalationEventDefinition = createEscalationEventDefinitionForEscalationHandler(escalationEventDefinitionElement, boundaryEventActivity, cancelActivity);
		addEscalationEventDefinition(boundaryEventActivity.EventScope, escalationEventDefinition, escalationEventDefinitionElement);

		foreach (BpmnParseListener parseListener in parseListeners)
		{
		  parseListener.parseBoundaryEscalationEventDefinition(escalationEventDefinitionElement, cancelActivity, boundaryEventActivity);
		}
	  }

	  /// <summary>
	  /// Find the referenced escalation of the given escalation event definition.
	  /// Add errors if the referenced escalation not found.
	  /// </summary>
	  /// <returns> referenced escalation or <code>null</code>, if referenced escalation not found </returns>
	  protected internal virtual Escalation findEscalationForEscalationEventDefinition(Element escalationEventDefinition)
	  {
		string escalationRef = escalationEventDefinition.attribute("escalationRef");
		if (string.ReferenceEquals(escalationRef, null))
		{
		  addError("escalationEventDefinition does not have required attribute 'escalationRef'", escalationEventDefinition);
		}
		else if (!escalations.ContainsKey(escalationRef))
		{
		  addError("could not find escalation with id '" + escalationRef + "'", escalationEventDefinition);
		}
		else
		{
		  return escalations[escalationRef];
		}
		return null;
	  }

	  protected internal virtual EscalationEventDefinition createEscalationEventDefinitionForEscalationHandler(Element escalationEventDefinitionElement, ActivityImpl escalationHandler, bool cancelActivity)
	  {
		EscalationEventDefinition escalationEventDefinition = new EscalationEventDefinition(escalationHandler, cancelActivity);

		string escalationRef = escalationEventDefinitionElement.attribute("escalationRef");
		if (!string.ReferenceEquals(escalationRef, null))
		{
		  if (!escalations.ContainsKey(escalationRef))
		  {
			addError("could not find escalation with id '" + escalationRef + "'", escalationEventDefinitionElement);
		  }
		  else
		  {
			Escalation escalation = escalations[escalationRef];
			escalationEventDefinition.EscalationCode = escalation.EscalationCode;
		  }
		}

		string escalationCodeVariable = escalationEventDefinitionElement.attributeNS(CAMUNDA_BPMN_EXTENSIONS_NS, "escalationCodeVariable");
		if (!string.ReferenceEquals(escalationCodeVariable, null))
		{
		  escalationEventDefinition.EscalationCodeVariable = escalationCodeVariable;
		}

		return escalationEventDefinition;
	  }

	  protected internal virtual void addEscalationEventDefinition(ScopeImpl catchingScope, EscalationEventDefinition escalationEventDefinition, Element element)
	  {
		// ensure there is only one escalation handler (e.g. escalation boundary event, escalation event subprocess) what can catch the escalation event
		foreach (EscalationEventDefinition existingEscalationEventDefinition in catchingScope.Properties.get(BpmnProperties.ESCALATION_EVENT_DEFINITIONS))
		{

		  if (existingEscalationEventDefinition.EscalationHandler.SubProcessScope && escalationEventDefinition.EscalationHandler.SubProcessScope)
		  {

			if (string.ReferenceEquals(existingEscalationEventDefinition.EscalationCode, null) && string.ReferenceEquals(escalationEventDefinition.EscalationCode, null))
			{
			  addError("The same scope can not contains more than one escalation event subprocess without escalation code. " + "An escalation event subprocess without escalation code catch all escalation events.", element);
			}
			else if (string.ReferenceEquals(existingEscalationEventDefinition.EscalationCode, null) || string.ReferenceEquals(escalationEventDefinition.EscalationCode, null))
			{
			  addError("The same scope can not contains an escalation event subprocess without escalation code and another one with escalation code. " + "The escalation event subprocess without escalation code catch all escalation events.", element);
			}
			else if (existingEscalationEventDefinition.EscalationCode.Equals(escalationEventDefinition.EscalationCode))
			{
			  addError("multiple escalation event subprocesses with the same escalationCode '" + escalationEventDefinition.EscalationCode + "' are not supported on same scope", element);
			}
		  }
		  else if (!existingEscalationEventDefinition.EscalationHandler.SubProcessScope && !escalationEventDefinition.EscalationHandler.SubProcessScope)
		  {

			if (string.ReferenceEquals(existingEscalationEventDefinition.EscalationCode, null) && string.ReferenceEquals(escalationEventDefinition.EscalationCode, null))
			{
			  addError("The same scope can not contains more than one escalation boundary event without escalation code. " + "An escalation boundary event without escalation code catch all escalation events.", element);
			}
			else if (string.ReferenceEquals(existingEscalationEventDefinition.EscalationCode, null) || string.ReferenceEquals(escalationEventDefinition.EscalationCode, null))
			{
			  addError("The same scope can not contains an escalation boundary event without escalation code and another one with escalation code. " + "The escalation boundary event without escalation code catch all escalation events.", element);
			}
			else if (existingEscalationEventDefinition.EscalationCode.Equals(escalationEventDefinition.EscalationCode))
			{
			  addError("multiple escalation boundary events with the same escalationCode '" + escalationEventDefinition.EscalationCode + "' are not supported on same scope", element);
			}
		  }
		}

		catchingScope.Properties.addListItem(BpmnProperties.ESCALATION_EVENT_DEFINITIONS, escalationEventDefinition);
	  }

	  protected internal virtual void addTimerDeclaration(ScopeImpl scope, TimerDeclarationImpl timerDeclaration)
	  {
		scope.Properties.putMapEntry(BpmnProperties.TIMER_DECLARATIONS, timerDeclaration.ActivityId, timerDeclaration);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") protected void addVariableDeclaration(ScopeImpl scope, org.camunda.bpm.engine.impl.variable.VariableDeclaration variableDeclaration)
	  protected internal virtual void addVariableDeclaration(ScopeImpl scope, VariableDeclaration variableDeclaration)
	  {
		IList<VariableDeclaration> variableDeclarations = (IList<VariableDeclaration>) scope.getProperty(PROPERTYNAME_VARIABLE_DECLARATIONS);
		if (variableDeclarations == null)
		{
		  variableDeclarations = new List<VariableDeclaration>();
		  scope.setProperty(PROPERTYNAME_VARIABLE_DECLARATIONS, variableDeclarations);
		}
		variableDeclarations.Add(variableDeclaration);
	  }

	  /// <summary>
	  /// Parses the given element as conditional boundary event.
	  /// </summary>
	  /// <param name="element"> the XML element which contains the conditional event information </param>
	  /// <param name="interrupting"> indicates if the event is interrupting or not </param>
	  /// <param name="conditionalActivity"> the conditional event activity </param>
	  /// <returns> the boundary conditional event behavior which contains the condition </returns>
	  public virtual BoundaryConditionalEventActivityBehavior parseBoundaryConditionalEventDefinition(Element element, bool interrupting, ActivityImpl conditionalActivity)
	  {
		conditionalActivity.Properties.set(BpmnProperties.TYPE, ActivityTypes.BOUNDARY_CONDITIONAL);

		ConditionalEventDefinition conditionalEventDefinition = parseConditionalEventDefinition(element, conditionalActivity);
		conditionalEventDefinition.Interrupting = interrupting;
		addEventSubscriptionDeclaration(conditionalEventDefinition, conditionalActivity.EventScope, element);

		foreach (BpmnParseListener parseListener in parseListeners)
		{
		  parseListener.parseBoundaryConditionalEventDefinition(element, interrupting, conditionalActivity);
		}

		return new BoundaryConditionalEventActivityBehavior(conditionalEventDefinition);
	  }

	  /// <summary>
	  /// Parses the given element as intermediate conditional event.
	  /// </summary>
	  /// <param name="element"> the XML element which contains the conditional event information </param>
	  /// <param name="conditionalActivity"> the conditional event activity </param>
	  /// <returns> returns the conditional activity with the parsed information </returns>
	  public virtual ConditionalEventDefinition parseIntermediateConditionalEventDefinition(Element element, ActivityImpl conditionalActivity)
	  {
		conditionalActivity.Properties.set(BpmnProperties.TYPE, ActivityTypes.INTERMEDIATE_EVENT_CONDITIONAL);

		ConditionalEventDefinition conditionalEventDefinition = parseConditionalEventDefinition(element, conditionalActivity);
		addEventSubscriptionDeclaration(conditionalEventDefinition, conditionalActivity.EventScope, element);

		foreach (BpmnParseListener parseListener in parseListeners)
		{
		  parseListener.parseIntermediateConditionalEventDefinition(element, conditionalActivity);
		}

		return conditionalEventDefinition;
	  }

	  /// <summary>
	  /// Parses the given element as conditional start event of an event subprocess.
	  /// </summary>
	  /// <param name="element"> the XML element which contains the conditional event information </param>
	  /// <param name="interrupting"> indicates if the event is interrupting or not </param>
	  /// <param name="conditionalActivity"> the conditional event activity
	  /// @return </param>
	  public virtual ConditionalEventDefinition parseConditionalStartEventForEventSubprocess(Element element, ActivityImpl conditionalActivity, bool interrupting)
	  {
		conditionalActivity.Properties.set(BpmnProperties.TYPE, ActivityTypes.START_EVENT_CONDITIONAL);

		ConditionalEventDefinition conditionalEventDefinition = parseConditionalEventDefinition(element, conditionalActivity);
		conditionalEventDefinition.Interrupting = interrupting;
		addEventSubscriptionDeclaration(conditionalEventDefinition, conditionalActivity.EventScope, element);

		foreach (BpmnParseListener parseListener in parseListeners)
		{
		  parseListener.parseConditionalStartEventForEventSubprocess(element, conditionalActivity, interrupting);
		}

		return conditionalEventDefinition;
	  }

	  /// <summary>
	  /// Parses the given element and returns an ConditionalEventDefinition object.
	  /// </summary>
	  /// <param name="element"> the XML element which contains the conditional event information </param>
	  /// <param name="conditionalActivity"> the conditional event activity </param>
	  /// <returns> the conditional event definition which was parsed </returns>
	  protected internal virtual ConditionalEventDefinition parseConditionalEventDefinition(Element element, ActivityImpl conditionalActivity)
	  {
		ConditionalEventDefinition conditionalEventDefinition = null;

		Element conditionExprElement = element.element(CONDITION);
		if (conditionExprElement != null)
		{
		  Condition condition = parseConditionExpression(conditionExprElement);
		  conditionalEventDefinition = new ConditionalEventDefinition(condition, conditionalActivity);

		  string expression = conditionExprElement.Text.Trim();
		  conditionalEventDefinition.ConditionAsString = expression;

		  conditionalActivity.ProcessDefinition.Properties.set(BpmnProperties.HAS_CONDITIONAL_EVENTS, true);

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final String variableName = element.attributeNS(CAMUNDA_BPMN_EXTENSIONS_NS, "variableName");
		  string variableName = element.attributeNS(CAMUNDA_BPMN_EXTENSIONS_NS, "variableName");
		  conditionalEventDefinition.VariableName = variableName;

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final String variableEvents = element.attributeNS(CAMUNDA_BPMN_EXTENSIONS_NS, "variableEvents");
		  string variableEvents = element.attributeNS(CAMUNDA_BPMN_EXTENSIONS_NS, "variableEvents");
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final List<String> variableEventsList = parseCommaSeparatedList(variableEvents);
		  IList<string> variableEventsList = parseCommaSeparatedList(variableEvents);
		  conditionalEventDefinition.VariableEvents = new HashSet<string>(variableEventsList);

		  foreach (string variableEvent in variableEventsList)
		  {
			if (!VARIABLE_EVENTS.Contains(variableEvent))
			{
			  addWarning("Variable event: " + variableEvent + " is not valid. Possible variable change events are: " + Arrays.ToString(VARIABLE_EVENTS.ToArray()), element);
			}
		  }

		}
		else
		{
		  addError("Conditional event must contain an expression for evaluation.", element);
		}

		return conditionalEventDefinition;
	  }

	  /// <summary>
	  /// Parses a subprocess (formally known as an embedded subprocess): a
	  /// subprocess defined within another process definition.
	  /// </summary>
	  /// <param name="subProcessElement">
	  ///          The XML element corresponding with the subprocess definition </param>
	  /// <param name="scope">
	  ///          The current scope on which the subprocess is defined. </param>
	  public virtual ActivityImpl parseSubProcess(Element subProcessElement, ScopeImpl scope)
	  {
		ActivityImpl subProcessActivity = createActivityOnScope(subProcessElement, scope);
		subProcessActivity.SubProcessScope = true;

		parseAsynchronousContinuationForActivity(subProcessElement, subProcessActivity);

		bool? isTriggeredByEvent = parseBooleanAttribute(subProcessElement.attribute("triggeredByEvent"), false);
		subProcessActivity.Properties.set(BpmnProperties.TRIGGERED_BY_EVENT, isTriggeredByEvent);
		subProcessActivity.setProperty(PROPERTYNAME_CONSUMES_COMPENSATION, !isTriggeredByEvent);

		subProcessActivity.Scope = true;
		if (isTriggeredByEvent.Value)
		{
		  subProcessActivity.ActivityBehavior = new EventSubProcessActivityBehavior();
		  subProcessActivity.EventScope = scope;
		}
		else
		{
		  subProcessActivity.ActivityBehavior = new SubProcessActivityBehavior();
		}
		parseScope(subProcessElement, subProcessActivity);

		foreach (BpmnParseListener parseListener in parseListeners)
		{
		  parseListener.parseSubProcess(subProcessElement, scope, subProcessActivity);
		}
		return subProcessActivity;
	  }

	  protected internal virtual ActivityImpl parseTransaction(Element transactionElement, ScopeImpl scope)
	  {
		ActivityImpl activity = createActivityOnScope(transactionElement, scope);

		parseAsynchronousContinuationForActivity(transactionElement, activity);

		activity.Scope = true;
		activity.SubProcessScope = true;
		activity.ActivityBehavior = new SubProcessActivityBehavior();
		activity.Properties.set(BpmnProperties.TRIGGERED_BY_EVENT, false);
		parseScope(transactionElement, activity);

		foreach (BpmnParseListener parseListener in parseListeners)
		{
		  parseListener.parseTransaction(transactionElement, scope, activity);
		}
		return activity;
	  }

	  /// <summary>
	  /// Parses a call activity (currently only supporting calling subprocesses).
	  /// </summary>
	  /// <param name="callActivityElement">
	  ///          The XML element defining the call activity </param>
	  /// <param name="scope">
	  ///          The current scope on which the call activity is defined. </param>
	  public virtual ActivityImpl parseCallActivity(Element callActivityElement, ScopeImpl scope, bool isMultiInstance)
	  {
		ActivityImpl activity = createActivityOnScope(callActivityElement, scope);

		// parse async
		parseAsynchronousContinuationForActivity(callActivityElement, activity);

		// parse definition key (and behavior)
		string calledElement = callActivityElement.attribute("calledElement");
		string caseRef = callActivityElement.attributeNS(CAMUNDA_BPMN_EXTENSIONS_NS, "caseRef");
		string className = callActivityElement.attributeNS(CAMUNDA_BPMN_EXTENSIONS_NS, PROPERTYNAME_VARIABLE_MAPPING_CLASS);
		string delegateExpression = callActivityElement.attributeNS(CAMUNDA_BPMN_EXTENSIONS_NS, PROPERTYNAME_VARIABLE_MAPPING_DELEGATE_EXPRESSION);

		if (string.ReferenceEquals(calledElement, null) && string.ReferenceEquals(caseRef, null))
		{
		  addError("Missing attribute 'calledElement' or 'caseRef'", callActivityElement);
		}
		else if (!string.ReferenceEquals(calledElement, null) && !string.ReferenceEquals(caseRef, null))
		{
		  addError("The attributes 'calledElement' or 'caseRef' cannot be used together: Use either 'calledElement' or 'caseRef'", callActivityElement);
		}

		string bindingAttributeName = "calledElementBinding";
		string versionAttributeName = "calledElementVersion";
		string versionTagAttributeName = "calledElementVersionTag";
		string tenantIdAttributeName = "calledElementTenantId";

		string deploymentId = deployment_Renamed.Id;

		CallableElement callableElement = new CallableElement();
		callableElement.DeploymentId = deploymentId;

		CallableElementActivityBehavior behavior = null;

		if (!string.ReferenceEquals(calledElement, null))
		{
		  if (!string.ReferenceEquals(className, null))
		  {
			  behavior = new CallActivityBehavior(className);
		  }
		  else if (!string.ReferenceEquals(delegateExpression, null))
		  {
			 Expression exp = expressionManager.createExpression(delegateExpression);
			 behavior = new CallActivityBehavior(exp);
		  }
		  else
		  {
			behavior = new CallActivityBehavior();
		  }
		  ParameterValueProvider definitionKeyProvider = createParameterValueProvider(calledElement, expressionManager);
		  callableElement.DefinitionKeyValueProvider = definitionKeyProvider;

		}
		else
		{
		  behavior = new CaseCallActivityBehavior();
		  ParameterValueProvider definitionKeyProvider = createParameterValueProvider(caseRef, expressionManager);
		  callableElement.DefinitionKeyValueProvider = definitionKeyProvider;
		  bindingAttributeName = "caseBinding";
		  versionAttributeName = "caseVersion";
		  tenantIdAttributeName = "caseTenantId";
		}

		behavior.CallableElement = callableElement;

		// parse binding
		parseBinding(callActivityElement, activity, callableElement, bindingAttributeName);

		// parse version
		parseVersion(callActivityElement, activity, callableElement, bindingAttributeName, versionAttributeName);

		// parse versionTag
		parseVersionTag(callActivityElement, activity, callableElement, bindingAttributeName, versionTagAttributeName);

		// parse tenant id
		parseTenantId(callActivityElement, activity, callableElement, tenantIdAttributeName);

		// parse input parameter
		parseInputParameter(callActivityElement, callableElement);

		// parse output parameter
		parseOutputParameter(callActivityElement, activity, callableElement);

		if (!isMultiInstance)
		{
		  // turn activity into a scope unless it is a multi instance activity, in
		  // that case this
		  // is not necessary because there is already the multi instance body scope
		  // and concurrent
		  // child executions are sufficient
		  activity.Scope = true;
		}
		activity.ActivityBehavior = behavior;

		parseExecutionListenersOnScope(callActivityElement, activity);

		foreach (BpmnParseListener parseListener in parseListeners)
		{
		  parseListener.parseCallActivity(callActivityElement, scope, activity);
		}
		return activity;
	  }

	  protected internal virtual void parseBinding(Element callActivityElement, ActivityImpl activity, BaseCallableElement callableElement, string bindingAttributeName)
	  {
		string binding = callActivityElement.attributeNS(CAMUNDA_BPMN_EXTENSIONS_NS, bindingAttributeName);

		if (CallableElementBinding.DEPLOYMENT.Value.Equals(binding))
		{
		  callableElement.Binding = CallableElementBinding.DEPLOYMENT;
		}
		else if (CallableElementBinding.LATEST.Value.Equals(binding))
		{
		  callableElement.Binding = CallableElementBinding.LATEST;
		}
		else if (CallableElementBinding.VERSION.Value.Equals(binding))
		{
		  callableElement.Binding = CallableElementBinding.VERSION;
		}
		else if (CallableElementBinding.VERSION_TAG.Value.Equals(binding))
		{
		  callableElement.Binding = CallableElementBinding.VERSION_TAG;
		}
	  }

	  protected internal virtual void parseTenantId(Element callingActivityElement, ActivityImpl activity, BaseCallableElement callableElement, string attrName)
	  {
		ParameterValueProvider tenantIdValueProvider;

		string tenantId = callingActivityElement.attributeNS(CAMUNDA_BPMN_EXTENSIONS_NS, attrName);
		if (!string.ReferenceEquals(tenantId, null) && tenantId.Length > 0)
		{
		  tenantIdValueProvider = createParameterValueProvider(tenantId, expressionManager);
		}
		else
		{
		  tenantIdValueProvider = new DefaultCallableElementTenantIdProvider();
		}

		callableElement.TenantIdProvider = tenantIdValueProvider;
	  }

	  protected internal virtual void parseVersion(Element callingActivityElement, ActivityImpl activity, BaseCallableElement callableElement, string bindingAttributeName, string versionAttributeName)
	  {
		string version = null;

		CallableElementBinding binding = callableElement.Binding;
		version = callingActivityElement.attributeNS(CAMUNDA_BPMN_EXTENSIONS_NS, versionAttributeName);

		if (binding != null && binding.Equals(CallableElementBinding.VERSION) && string.ReferenceEquals(version, null))
		{
		  addError("Missing attribute '" + versionAttributeName + "' when '" + bindingAttributeName + "' has value '" + CallableElementBinding.VERSION.Value + "'", callingActivityElement);
		}

		ParameterValueProvider versionProvider = createParameterValueProvider(version, expressionManager);
		callableElement.VersionValueProvider = versionProvider;
	  }

	  protected internal virtual void parseVersionTag(Element callingActivityElement, ActivityImpl activity, BaseCallableElement callableElement, string bindingAttributeName, string versionTagAttributeName)
	  {
		string versionTag = null;

		CallableElementBinding binding = callableElement.Binding;
		versionTag = callingActivityElement.attributeNS(CAMUNDA_BPMN_EXTENSIONS_NS, versionTagAttributeName);

		if (binding != null && binding.Equals(CallableElementBinding.VERSION_TAG) && string.ReferenceEquals(versionTag, null))
		{
		  addError("Missing attribute '" + versionTagAttributeName + "' when '" + bindingAttributeName + "' has value '" + CallableElementBinding.VERSION_TAG.Value + "'", callingActivityElement);
		}

		ParameterValueProvider versionTagProvider = createParameterValueProvider(versionTag, expressionManager);
		callableElement.VersionTagValueProvider = versionTagProvider;
	  }

	  protected internal virtual void parseInputParameter(Element elementWithParameters, CallableElement callableElement)
	  {
		Element extensionsElement = elementWithParameters.element("extensionElements");

		if (extensionsElement != null)
		{
		  // input data elements
		  foreach (Element inElement in extensionsElement.elementsNS(CAMUNDA_BPMN_EXTENSIONS_NS, "in"))
		  {

			string businessKey = inElement.attribute("businessKey");

			if (!string.ReferenceEquals(businessKey, null) && businessKey.Length > 0)
			{
			  ParameterValueProvider businessKeyValueProvider = createParameterValueProvider(businessKey, expressionManager);
			  callableElement.BusinessKeyValueProvider = businessKeyValueProvider;

			}
			else
			{

			  CallableElementParameter parameter = parseCallableElementProvider(inElement);

			  if (attributeValueEquals(inElement, "local", TRUE))
			  {
				parameter.ReadLocal = true;
			  }

			  callableElement.addInput(parameter);
			}
		  }
		}
	  }

	  protected internal virtual void parseOutputParameter(Element callActivityElement, ActivityImpl activity, CallableElement callableElement)
	  {
		Element extensionsElement = callActivityElement.element("extensionElements");

		if (extensionsElement != null)
		{
		  // output data elements
		  foreach (Element outElement in extensionsElement.elementsNS(CAMUNDA_BPMN_EXTENSIONS_NS, "out"))
		  {

			CallableElementParameter parameter = parseCallableElementProvider(outElement);

			if (attributeValueEquals(outElement, "local", TRUE))
			{
			  callableElement.addOutputLocal(parameter);
			}
			else
			{
			  callableElement.addOutput(parameter);
			}

		  }
		}
	  }

	  protected internal virtual bool attributeValueEquals(Element element, string attribute, string comparisonValue)
	  {
		string value = element.attribute(attribute);

		return comparisonValue.Equals(value);
	  }

	  protected internal virtual CallableElementParameter parseCallableElementProvider(Element parameterElement)
	  {
		CallableElementParameter parameter = new CallableElementParameter();

		string variables = parameterElement.attribute("variables");

		if (ALL.Equals(variables))
		{
		  parameter.AllVariables = true;
		}
		else
		{
		  bool strictValidation = !Context.ProcessEngineConfiguration.DisableStrictCallActivityValidation;

		  ParameterValueProvider sourceValueProvider = new NullValueProvider();

		  string source = parameterElement.attribute("source");
		  if (!string.ReferenceEquals(source, null))
		  {
			if (source.Length > 0)
			{
			  sourceValueProvider = new ConstantValueProvider(source);
			}
			else
			{
			  if (strictValidation)
			  {
				addError("Empty attribute 'source' when passing variables", parameterElement);
			  }
			  else
			  {
				source = null;
			  }
			}
		  }

		  if (string.ReferenceEquals(source, null))
		  {
			source = parameterElement.attribute("sourceExpression");

			if (!string.ReferenceEquals(source, null))
			{
			  if (source.Length > 0)
			  {
				Expression expression = expressionManager.createExpression(source);
				sourceValueProvider = new ElValueProvider(expression);
			  }
			  else if (strictValidation)
			  {
				addError("Empty attribute 'sourceExpression' when passing variables", parameterElement);
			  }
			}
		  }

		  if (strictValidation && string.ReferenceEquals(source, null))
		  {
			addError("Missing parameter 'source' or 'sourceExpression' when passing variables", parameterElement);
		  }

		  parameter.SourceValueProvider = sourceValueProvider;

		  string target = parameterElement.attribute("target");
		  if ((strictValidation || !string.ReferenceEquals(source, null) && source.Length > 0) && string.ReferenceEquals(target, null))
		  {
			addError("Missing attribute 'target' when attribute 'source' or 'sourceExpression' is set", parameterElement);
		  }
		  else if (strictValidation && !string.ReferenceEquals(target, null) && target.Length == 0)
		  {
			addError("Empty attribute 'target' when attribute 'source' or 'sourceExpression' is set", parameterElement);
		  }
		  parameter.Target = target;
		}

		return parameter;
	  }

	  /// <summary>
	  /// Parses the properties of an element (if any) that can contain properties
	  /// (processes, activities, etc.)
	  /// 
	  /// Returns true if property subelemens are found.
	  /// </summary>
	  /// <param name="element">
	  ///          The element that can contain properties. </param>
	  /// <param name="activity">
	  ///          The activity where the property declaration is done. </param>
	  public virtual void parseProperties(Element element, ActivityImpl activity)
	  {
		IList<Element> propertyElements = element.elements("property");
		foreach (Element propertyElement in propertyElements)
		{
		  parseProperty(propertyElement, activity);
		}
	  }

	  /// <summary>
	  /// Parses one property definition.
	  /// </summary>
	  /// <param name="propertyElement">
	  ///          The 'property' element that defines how a property looks like and
	  ///          is handled. </param>
	  public virtual void parseProperty(Element propertyElement, ActivityImpl activity)
	  {
		string id = propertyElement.attribute("id");
		string name = propertyElement.attribute("name");

		// If name isn't given, use the id as name
		if (string.ReferenceEquals(name, null))
		{
		  if (string.ReferenceEquals(id, null))
		  {
			addError("Invalid property usage on line " + propertyElement.Line + ": no id or name specified.", propertyElement);
		  }
		  else
		  {
			name = id;
		  }
		}

		string type = null;
		parsePropertyCustomExtensions(activity, propertyElement, name, type);
	  }

	  /// <summary>
	  /// Parses the custom extensions for properties.
	  /// </summary>
	  /// <param name="activity">
	  ///          The activity where the property declaration is done. </param>
	  /// <param name="propertyElement">
	  ///          The 'property' element defining the property. </param>
	  /// <param name="propertyName">
	  ///          The name of the property. </param>
	  /// <param name="propertyType">
	  ///          The type of the property. </param>
	  public virtual void parsePropertyCustomExtensions(ActivityImpl activity, Element propertyElement, string propertyName, string propertyType)
	  {

		if (string.ReferenceEquals(propertyType, null))
		{
		  string type = propertyElement.attributeNS(CAMUNDA_BPMN_EXTENSIONS_NS, TYPE);
		  propertyType = !string.ReferenceEquals(type, null) ? type : "string"; // default is string
		}

		VariableDeclaration variableDeclaration = new VariableDeclaration(propertyName, propertyType);
		addVariableDeclaration(activity, variableDeclaration);
		activity.Scope = true;

		string src = propertyElement.attributeNS(CAMUNDA_BPMN_EXTENSIONS_NS, "src");
		if (!string.ReferenceEquals(src, null))
		{
		  variableDeclaration.SourceVariableName = src;
		}

		string srcExpr = propertyElement.attributeNS(CAMUNDA_BPMN_EXTENSIONS_NS, "srcExpr");
		if (!string.ReferenceEquals(srcExpr, null))
		{
		  Expression sourceExpression = expressionManager.createExpression(srcExpr);
		  variableDeclaration.SourceExpression = sourceExpression;
		}

		string dst = propertyElement.attributeNS(CAMUNDA_BPMN_EXTENSIONS_NS, "dst");
		if (!string.ReferenceEquals(dst, null))
		{
		  variableDeclaration.DestinationVariableName = dst;
		}

		string destExpr = propertyElement.attributeNS(CAMUNDA_BPMN_EXTENSIONS_NS, "dstExpr");
		if (!string.ReferenceEquals(destExpr, null))
		{
		  Expression destinationExpression = expressionManager.createExpression(destExpr);
		  variableDeclaration.DestinationExpression = destinationExpression;
		}

		string link = propertyElement.attributeNS(CAMUNDA_BPMN_EXTENSIONS_NS, "link");
		if (!string.ReferenceEquals(link, null))
		{
		  variableDeclaration.Link = link;
		}

		string linkExpr = propertyElement.attributeNS(CAMUNDA_BPMN_EXTENSIONS_NS, "linkExpr");
		if (!string.ReferenceEquals(linkExpr, null))
		{
		  Expression linkExpression = expressionManager.createExpression(linkExpr);
		  variableDeclaration.LinkExpression = linkExpression;
		}

		foreach (BpmnParseListener parseListener in parseListeners)
		{
		  parseListener.parseProperty(propertyElement, variableDeclaration, activity);
		}
	  }

	  /// <summary>
	  /// Parses all sequence flow of a scope.
	  /// </summary>
	  /// <param name="processElement">
	  ///          The 'process' element wherein the sequence flow are defined. </param>
	  /// <param name="scope">
	  ///          The scope to which the sequence flow must be added. </param>
	  /// <param name="compensationHandlers"> </param>
	  public virtual void parseSequenceFlow(Element processElement, ScopeImpl scope, IDictionary<string, Element> compensationHandlers)
	  {
		foreach (Element sequenceFlowElement in processElement.elements("sequenceFlow"))
		{

		  string id = sequenceFlowElement.attribute("id");
		  string sourceRef = sequenceFlowElement.attribute("sourceRef");
		  string destinationRef = sequenceFlowElement.attribute("targetRef");

		  // check if destination is a throwing link event (event source) which mean
		  // we have
		  // to target the catching link event (event target) here:
		  if (eventLinkSources.ContainsKey(destinationRef))
		  {
			string linkName = eventLinkSources[destinationRef];
			destinationRef = eventLinkTargets[linkName];
			if (string.ReferenceEquals(destinationRef, null))
			{
			  addError("sequence flow points to link event source with name '" + linkName + "' but no event target with that name exists. Most probably your link events are not configured correctly.", sequenceFlowElement);
			  // we cannot do anything useful now
			  return;
			}
			// Reminder: Maybe we should log a warning if we use intermediate link
			// events which are not used?
			// e.g. we have a catching event without the corresponding throwing one.
			// not done for the moment as it does not break executability
		  }

		  // Implicit check: sequence flow cannot cross (sub) process boundaries: we
		  // don't do a processDefinition.findActivity here
		  ActivityImpl sourceActivity = scope.findActivityAtLevelOfSubprocess(sourceRef);
		  ActivityImpl destinationActivity = scope.findActivityAtLevelOfSubprocess(destinationRef);

		  if ((sourceActivity == null && compensationHandlers.ContainsKey(sourceRef)) || (sourceActivity != null && sourceActivity.CompensationHandler))
		  {
			addError("Invalid outgoing sequence flow of compensation activity '" + sourceRef + "'. A compensation activity should not have an incoming or outgoing sequence flow.", sequenceFlowElement);
		  }
		  else if ((destinationActivity == null && compensationHandlers.ContainsKey(destinationRef)) || (destinationActivity != null && destinationActivity.CompensationHandler))
		  {
			addError("Invalid incoming sequence flow of compensation activity '" + destinationRef + "'. A compensation activity should not have an incoming or outgoing sequence flow.", sequenceFlowElement);
		  }
		  else if (sourceActivity == null)
		  {
			addError("Invalid source '" + sourceRef + "' of sequence flow '" + id + "'", sequenceFlowElement);
		  }
		  else if (destinationActivity == null)
		  {
			addError("Invalid destination '" + destinationRef + "' of sequence flow '" + id + "'", sequenceFlowElement);
		  }
		  else if (sourceActivity.ActivityBehavior is EventBasedGatewayActivityBehavior)
		  {
			// ignore
		  }
		  else if (destinationActivity.ActivityBehavior is IntermediateCatchEventActivityBehavior && (destinationActivity.EventScope != null) && (destinationActivity.EventScope.ActivityBehavior is EventBasedGatewayActivityBehavior))
		  {
			addError("Invalid incoming sequenceflow for intermediateCatchEvent with id '" + destinationActivity.Id + "' connected to an event-based gateway.", sequenceFlowElement);
		  }
		  else if (sourceActivity.ActivityBehavior is SubProcessActivityBehavior && sourceActivity.TriggeredByEvent)
		  {
			addError("Invalid outgoing sequence flow of event subprocess", sequenceFlowElement);
		  }
		  else if (destinationActivity.ActivityBehavior is SubProcessActivityBehavior && destinationActivity.TriggeredByEvent)
		  {
			addError("Invalid incoming sequence flow of event subprocess", sequenceFlowElement);
		  }
		  else
		  {

			if (getMultiInstanceScope(sourceActivity) != null)
			{
			  sourceActivity = getMultiInstanceScope(sourceActivity);
			}
			if (getMultiInstanceScope(destinationActivity) != null)
			{
			  destinationActivity = getMultiInstanceScope(destinationActivity);
			}

			TransitionImpl transition = sourceActivity.createOutgoingTransition(id);
			sequenceFlows[id] = transition;
			transition.setProperty("name", sequenceFlowElement.attribute("name"));
			transition.setProperty("documentation", parseDocumentation(sequenceFlowElement));
			transition.setDestination(destinationActivity);
			parseSequenceFlowConditionExpression(sequenceFlowElement, transition);
			parseExecutionListenersOnTransition(sequenceFlowElement, transition);

			foreach (BpmnParseListener parseListener in parseListeners)
			{
			  parseListener.parseSequenceFlow(sequenceFlowElement, scope, transition);
			}
		  }
		}
	  }

	  /// <summary>
	  /// Parses a condition expression on a sequence flow.
	  /// </summary>
	  /// <param name="seqFlowElement">
	  ///          The 'sequenceFlow' element that can contain a condition. </param>
	  /// <param name="seqFlow">
	  ///          The sequenceFlow object representation to which the condition must
	  ///          be added. </param>
	  public virtual void parseSequenceFlowConditionExpression(Element seqFlowElement, TransitionImpl seqFlow)
	  {
		Element conditionExprElement = seqFlowElement.element(CONDITION_EXPRESSION);
		if (conditionExprElement != null)
		{
		  Condition condition = parseConditionExpression(conditionExprElement);
		  seqFlow.setProperty(PROPERTYNAME_CONDITION_TEXT, conditionExprElement.Text.Trim());
		  seqFlow.setProperty(PROPERTYNAME_CONDITION, condition);
		}
	  }

	  protected internal virtual Condition parseConditionExpression(Element conditionExprElement)
	  {
		string expression = conditionExprElement.Text.Trim();
		string type = conditionExprElement.attributeNS(XSI_NS, TYPE);
		string language = conditionExprElement.attribute(PROPERTYNAME_LANGUAGE);
		string resource = conditionExprElement.attributeNS(CAMUNDA_BPMN_EXTENSIONS_NS, PROPERTYNAME_RESOURCE);
		if (!string.ReferenceEquals(type, null))
		{
		  string value = type.Contains(":") ? resolveName(type) : BpmnParser.BPMN20_NS + ":" + type;
		  if (!value.Equals(ATTRIBUTEVALUE_T_FORMAL_EXPRESSION))
		  {
			addError("Invalid type, only tFormalExpression is currently supported", conditionExprElement);
		  }
		}
		Condition condition = null;
		if (string.ReferenceEquals(language, null))
		{
		  condition = new UelExpressionCondition(expressionManager.createExpression(expression));
		}
		else
		{
		  try
		  {
			ExecutableScript script = ScriptUtil.getScript(language, expression, resource, expressionManager);
			condition = new ScriptCondition(script);
		  }
		  catch (ProcessEngineException e)
		  {
			addError("Unable to process condition expression:" + e.Message, conditionExprElement);
		  }
		}
		return condition;
	  }

	  /// <summary>
	  /// Parses all execution-listeners on a scope.
	  /// </summary>
	  /// <param name="scopeElement">
	  ///          the XML element containing the scope definition. </param>
	  /// <param name="scope">
	  ///          the scope to add the executionListeners to. </param>
	  public virtual void parseExecutionListenersOnScope(Element scopeElement, ScopeImpl scope)
	  {
		Element extentionsElement = scopeElement.element("extensionElements");
		if (extentionsElement != null)
		{
		  IList<Element> listenerElements = extentionsElement.elementsNS(CAMUNDA_BPMN_EXTENSIONS_NS, "executionListener");
		  foreach (Element listenerElement in listenerElements)
		  {
			string eventName = listenerElement.attribute("event");
			if (isValidEventNameForScope(eventName, listenerElement))
			{
			  ExecutionListener listener = parseExecutionListener(listenerElement);
			  if (listener != null)
			  {
				scope.addExecutionListener(eventName, listener);
			  }
			}
		  }
		}
	  }

	  /// <summary>
	  /// Check if the given event name is valid. If not, an appropriate error is
	  /// added.
	  /// </summary>
	  protected internal virtual bool isValidEventNameForScope(string eventName, Element listenerElement)
	  {
		if (!string.ReferenceEquals(eventName, null) && eventName.Trim().Length > 0)
		{
		  if ("start".Equals(eventName) || "end".Equals(eventName))
		  {
			return true;
		  }
		  else
		  {
			addError("Attribute 'event' must be one of {start|end}", listenerElement);
		  }
		}
		else
		{
		  addError("Attribute 'event' is mandatory on listener", listenerElement);
		}
		return false;
	  }

	  public virtual void parseExecutionListenersOnTransition(Element activitiElement, TransitionImpl activity)
	  {
		Element extensionElements = activitiElement.element("extensionElements");
		if (extensionElements != null)
		{
		  IList<Element> listenerElements = extensionElements.elementsNS(CAMUNDA_BPMN_EXTENSIONS_NS, "executionListener");
		  foreach (Element listenerElement in listenerElements)
		  {
			ExecutionListener listener = parseExecutionListener(listenerElement);
			if (listener != null)
			{
			  // Since a transition only fires event 'take', we don't parse the
			  // event attribute, it is ignored
			  activity.addExecutionListener(listener);
			}
		  }
		}
	  }

	  /// <summary>
	  /// Parses an <seealso cref="ExecutionListener"/> implementation for the given
	  /// executionListener element.
	  /// </summary>
	  /// <param name="executionListenerElement">
	  ///          the XML element containing the executionListener definition. </param>
	  public virtual ExecutionListener parseExecutionListener(Element executionListenerElement)
	  {
		ExecutionListener executionListener = null;

		string className = executionListenerElement.attribute(PROPERTYNAME_CLASS);
		string expression = executionListenerElement.attribute(PROPERTYNAME_EXPRESSION);
		string delegateExpression = executionListenerElement.attribute(PROPERTYNAME_DELEGATE_EXPRESSION);
		Element scriptElement = executionListenerElement.elementNS(CAMUNDA_BPMN_EXTENSIONS_NS, "script");

		if (!string.ReferenceEquals(className, null))
		{
		  executionListener = new ClassDelegateExecutionListener(className, parseFieldDeclarations(executionListenerElement));
		}
		else if (!string.ReferenceEquals(expression, null))
		{
		  executionListener = new ExpressionExecutionListener(expressionManager.createExpression(expression));
		}
		else if (!string.ReferenceEquals(delegateExpression, null))
		{
		  executionListener = new DelegateExpressionExecutionListener(expressionManager.createExpression(delegateExpression), parseFieldDeclarations(executionListenerElement));
		}
		else if (scriptElement != null)
		{
		  try
		  {
			ExecutableScript executableScript = parseCamundaScript(scriptElement);
			if (executableScript != null)
			{
			  executionListener = new ScriptExecutionListener(executableScript);
			}
		  }
		  catch (BpmnParseException e)
		  {
			addError(e);
		  }
		}
		else
		{
		  addError("Element 'class', 'expression', 'delegateExpression' or 'script' is mandatory on executionListener", executionListenerElement);
		}
		return executionListener;
	  }

	  // Diagram interchange
	  // /////////////////////////////////////////////////////////////////

	  public virtual void parseDiagramInterchangeElements()
	  {
		// Multiple BPMNDiagram possible
		IList<Element> diagrams = rootElement.elementsNS(BPMN_DI_NS, "BPMNDiagram");
		if (diagrams.Count > 0)
		{
		  foreach (Element diagramElement in diagrams)
		  {
			parseBPMNDiagram(diagramElement);
		  }
		}
	  }

	  public virtual void parseBPMNDiagram(Element bpmndiagramElement)
	  {
		// Each BPMNdiagram needs to have exactly one BPMNPlane
		Element bpmnPlane = bpmndiagramElement.elementNS(BPMN_DI_NS, "BPMNPlane");
		if (bpmnPlane != null)
		{
		  parseBPMNPlane(bpmnPlane);
		}
	  }

	  public virtual void parseBPMNPlane(Element bpmnPlaneElement)
	  {
		string bpmnElement = bpmnPlaneElement.attribute("bpmnElement");
		if (!string.ReferenceEquals(bpmnElement, null) && !"".Equals(bpmnElement))
		{
		  // there seems to be only on process without collaboration
		  if (getProcessDefinition(bpmnElement) != null)
		  {
			getProcessDefinition(bpmnElement).GraphicalNotationDefined = true;
		  }

		  IList<Element> shapes = bpmnPlaneElement.elementsNS(BPMN_DI_NS, "BPMNShape");
		  foreach (Element shape in shapes)
		  {
			parseBPMNShape(shape);
		  }

		  IList<Element> edges = bpmnPlaneElement.elementsNS(BPMN_DI_NS, "BPMNEdge");
		  foreach (Element edge in edges)
		  {
			parseBPMNEdge(edge);
		  }

		}
		else
		{
		  addError("'bpmnElement' attribute is required on BPMNPlane ", bpmnPlaneElement);
		}
	  }

	  public virtual void parseBPMNShape(Element bpmnShapeElement)
	  {
		string bpmnElement = bpmnShapeElement.attribute("bpmnElement");

		if (!string.ReferenceEquals(bpmnElement, null) && !"".Equals(bpmnElement))
		{
		  // For collaborations, their are also shape definitions for the
		  // participants / processes
		  if (!string.ReferenceEquals(participantProcesses[bpmnElement], null))
		  {
			ProcessDefinitionEntity procDef = getProcessDefinition(participantProcesses[bpmnElement]);
			procDef.GraphicalNotationDefined = true;

			// The participation that references this process, has a bounds to be
			// rendered + a name as wel
			parseDIBounds(bpmnShapeElement, procDef.ParticipantProcess);
			return;
		  }

		  foreach (ProcessDefinitionEntity processDefinition in ProcessDefinitions)
		  {
			ActivityImpl activity = processDefinition.findActivity(bpmnElement);
			if (activity != null)
			{
			  parseDIBounds(bpmnShapeElement, activity);

			  // collapsed or expanded
			  string isExpanded = bpmnShapeElement.attribute("isExpanded");
			  if (!string.ReferenceEquals(isExpanded, null))
			  {
				activity.setProperty(PROPERTYNAME_ISEXPANDED, parseBooleanAttribute(isExpanded));
			  }
			}
			else
			{
			  Lane lane = processDefinition.getLaneForId(bpmnElement);

			  if (lane != null)
			  {
				// The shape represents a lane
				parseDIBounds(bpmnShapeElement, lane);
			  }
			  else if (!elementIds.Contains(bpmnElement))
			  { // It might not be an
															  // activity nor a
															  // lane, but it might
															  // still reference
															  // 'something'
				addError("Invalid reference in 'bpmnElement' attribute, activity " + bpmnElement + " not found", bpmnShapeElement);
			  }
			}
		  }
		}
		else
		{
		  addError("'bpmnElement' attribute is required on BPMNShape", bpmnShapeElement);
		}
	  }

	  protected internal virtual void parseDIBounds(Element bpmnShapeElement, HasDIBounds target)
	  {
		Element bounds = bpmnShapeElement.elementNS(BPMN_DC_NS, "Bounds");
		if (bounds != null)
		{
		  target.X = parseDoubleAttribute(bpmnShapeElement, "x", bounds.attribute("x"), true).Value;
		  target.Y = parseDoubleAttribute(bpmnShapeElement, "y", bounds.attribute("y"), true).Value;
		  target.Width = parseDoubleAttribute(bpmnShapeElement, "width", bounds.attribute("width"), true).Value;
		  target.Height = parseDoubleAttribute(bpmnShapeElement, "height", bounds.attribute("height"), true).Value;
		}
		else
		{
		  addError("'Bounds' element is required", bpmnShapeElement);
		}
	  }

	  public virtual void parseBPMNEdge(Element bpmnEdgeElement)
	  {
		string sequenceFlowId = bpmnEdgeElement.attribute("bpmnElement");
		if (!string.ReferenceEquals(sequenceFlowId, null) && !"".Equals(sequenceFlowId))
		{
		  if (sequenceFlows != null && sequenceFlows.ContainsKey(sequenceFlowId))
		  {

			TransitionImpl sequenceFlow = sequenceFlows[sequenceFlowId];
			IList<Element> waypointElements = bpmnEdgeElement.elementsNS(OMG_DI_NS, "waypoint");
			if (waypointElements.Count >= 2)
			{
			  IList<int> waypoints = new List<int>();
			  foreach (Element waypointElement in waypointElements)
			  {
				waypoints.Add(parseDoubleAttribute(waypointElement, "x", waypointElement.attribute("x"), true).Value);
				waypoints.Add(parseDoubleAttribute(waypointElement, "y", waypointElement.attribute("y"), true).Value);
			  }
			  sequenceFlow.Waypoints = waypoints;
			}
			else
			{
			  addError("Minimum 2 waypoint elements must be definted for a 'BPMNEdge'", bpmnEdgeElement);
			}
		  }
		  else if (!elementIds.Contains(sequenceFlowId))
		  { // it might not be a
															 // sequenceFlow but it
															 // might still
															 // reference
															 // 'something'
			addError("Invalid reference in 'bpmnElement' attribute, sequenceFlow " + sequenceFlowId + "not found", bpmnEdgeElement);
		  }
		}
		else
		{
		  addError("'bpmnElement' attribute is required on BPMNEdge", bpmnEdgeElement);
		}
	  }

	  // Getters, setters and Parser overridden operations
	  // ////////////////////////////////////////

	  public virtual IList<ProcessDefinitionEntity> ProcessDefinitions
	  {
		  get
		  {
			return processDefinitions;
		  }
	  }

	  public virtual ProcessDefinitionEntity getProcessDefinition(string processDefinitionKey)
	  {
		foreach (ProcessDefinitionEntity processDefinition in processDefinitions)
		{
		  if (processDefinition.Key.Equals(processDefinitionKey))
		  {
			return processDefinition;
		  }
		}
		return null;
	  }

	  public override BpmnParse name(string name)
	  {
		base.name(name);
		return this;
	  }

	  public override BpmnParse sourceInputStream(Stream inputStream)
	  {
		base.sourceInputStream(inputStream);
		return this;
	  }

	  public override BpmnParse sourceResource(string resource, ClassLoader classLoader)
	  {
		base.sourceResource(resource, classLoader);
		return this;
	  }

	  public override BpmnParse sourceResource(string resource)
	  {
		base.sourceResource(resource);
		return this;
	  }

	  public override BpmnParse sourceString(string @string)
	  {
		base.sourceString(@string);
		return this;
	  }

	  public override BpmnParse sourceUrl(string url)
	  {
		base.sourceUrl(url);
		return this;
	  }

	  public override BpmnParse sourceUrl(URL url)
	  {
		base.sourceUrl(url);
		return this;
	  }

	  public virtual bool? parseBooleanAttribute(string booleanText, bool defaultValue)
	  {
		if (string.ReferenceEquals(booleanText, null))
		{
		  return defaultValue;
		}
		else
		{
		  return parseBooleanAttribute(booleanText);
		}
	  }

	  public virtual bool? parseBooleanAttribute(string booleanText)
	  {
		if (TRUE.Equals(booleanText) || "enabled".Equals(booleanText) || "on".Equals(booleanText) || "active".Equals(booleanText) || "yes".Equals(booleanText))
		{
		  return true;
		}
		if ("false".Equals(booleanText) || "disabled".Equals(booleanText) || "off".Equals(booleanText) || "inactive".Equals(booleanText) || "no".Equals(booleanText))
		{
		  return false;
		}
		return null;
	  }

	  public virtual double? parseDoubleAttribute(Element element, string attributeName, string doubleText, bool required)
	  {
		if (required && (string.ReferenceEquals(doubleText, null) || "".Equals(doubleText)))
		{
		  addError(attributeName + " is required", element);
		}
		else
		{
		  try
		  {
			return double.Parse(doubleText);
		  }
		  catch (System.FormatException e)
		  {
			addError("Cannot parse " + attributeName + ": " + e.Message, element);
		  }
		}
		return -1.0;
	  }

	  protected internal virtual bool isStartable(Element element)
	  {
		return TRUE.Equals(element.attributeNS(CAMUNDA_BPMN_EXTENSIONS_NS, "isStartableInTasklist", TRUE), StringComparison.OrdinalIgnoreCase);
	  }

	  protected internal virtual bool isExclusive(Element element)
	  {
		return TRUE.Equals(element.attributeNS(CAMUNDA_BPMN_EXTENSIONS_NS, "exclusive", JobEntity.DEFAULT_EXCLUSIVE.ToString()));
	  }

	  protected internal virtual bool isAsyncBefore(Element element)
	  {
		return TRUE.Equals(element.attributeNS(CAMUNDA_BPMN_EXTENSIONS_NS, "async")) || TRUE.Equals(element.attributeNS(CAMUNDA_BPMN_EXTENSIONS_NS, "asyncBefore"));
	  }

	  protected internal virtual bool isAsyncAfter(Element element)
	  {
		return TRUE.Equals(element.attributeNS(CAMUNDA_BPMN_EXTENSIONS_NS, "asyncAfter"));
	  }

	  protected internal virtual bool isServiceTaskLike(Element element)
	  {

		return !string.ReferenceEquals(element.attributeNS(CAMUNDA_BPMN_EXTENSIONS_NS, PROPERTYNAME_CLASS), null) || !string.ReferenceEquals(element.attributeNS(CAMUNDA_BPMN_EXTENSIONS_NS, PROPERTYNAME_EXPRESSION), null) || !string.ReferenceEquals(element.attributeNS(CAMUNDA_BPMN_EXTENSIONS_NS, PROPERTYNAME_DELEGATE_EXPRESSION), null) || !string.ReferenceEquals(element.attributeNS(CAMUNDA_BPMN_EXTENSIONS_NS, TYPE), null) || hasConnector(element);
	  }

	  protected internal virtual bool hasConnector(Element element)
	  {
		Element extensionElements = element.element("extensionElements");
		return extensionElements != null && extensionElements.element("connector") != null;
	  }

//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: public Map<String, List<JobDeclaration<?, ?>>> getJobDeclarations()
	  public virtual IDictionary<string, IList<JobDeclaration<object, ?>>> JobDeclarations
	  {
		  get
		  {
			return jobDeclarations;
		  }
	  }

//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: public List<JobDeclaration<?, ?>> getJobDeclarationsByKey(String processDefinitionKey)
	  public virtual IList<JobDeclaration<object, ?>> getJobDeclarationsByKey(string processDefinitionKey)
	  {
		return jobDeclarations[processDefinitionKey];
	  }

	  // IoMappings ////////////////////////////////////////////////////////

	  protected internal virtual void parseActivityInputOutput(Element activityElement, ActivityImpl activity)
	  {
		Element extensionElements = activityElement.element("extensionElements");
		if (extensionElements != null)
		{
		  IoMapping inputOutput = null;
		  try
		  {
			inputOutput = parseInputOutput(extensionElements);
		  }
		  catch (BpmnParseException e)
		  {
			addError(e);
		  }

		  if (inputOutput != null)
		  {
			if (checkActivityInputOutputSupported(activityElement, activity, inputOutput))
			{

			  activity.IoMapping = inputOutput;

			  if (getMultiInstanceScope(activity) == null)
			  {
				// turn activity into a scope (->local, isolated scope for
				// variables) unless it is a multi instance activity, in that case
				// this
				// is not necessary because:
				// A scope is already created for the multi instance body which
				// isolates the local variables from other executions in the same
				// scope, and
				// * parallel: the individual concurrent executions are isolated
				// even if they are not scope themselves
				// * sequential: after each iteration local variables are purged
				activity.Scope = true;
			  }
			}
		  }
		}
	  }

	  protected internal virtual bool checkActivityInputOutputSupported(Element activityElement, ActivityImpl activity, IoMapping inputOutput)
	  {
		string tagName = activityElement.TagName;

		if (!(tagName.ToLower().Contains("task") || tagName.Contains("Event") || tagName.Equals("transaction") || tagName.Equals("subProcess") || tagName.Equals("callActivity")))
		{
		  addError("camunda:inputOutput mapping unsupported for element type '" + tagName + "'.", activityElement);
		  return false;
		}

		if (tagName.Equals("subProcess") && TRUE.Equals(activityElement.attribute("triggeredByEvent")))
		{
		  addError("camunda:inputOutput mapping unsupported for element type '" + tagName + "' with attribute 'triggeredByEvent = true'.", activityElement);
		  return false;
		}

		if (inputOutput.OutputParameters.Count > 0)
		{
		  return checkActivityOutputParameterSupported(activityElement, activity);
		}
		else
		{
		  return true;
		}
	  }

	  protected internal virtual bool checkActivityOutputParameterSupported(Element activityElement, ActivityImpl activity)
	  {
		string tagName = activityElement.TagName;

		if (tagName.Equals("endEvent"))
		{
		  addError("camunda:outputParameter not allowed for element type '" + tagName + "'.", activityElement);
		  return true;
		}
		else if (getMultiInstanceScope(activity) != null)
		{
		  addError("camunda:outputParameter not allowed for multi-instance constructs", activityElement);
		  return false;
		}
		else
		{
		  return true;
		}
	  }

	  protected internal virtual void ensureNoIoMappingDefined(Element element)
	  {
		Element inputOutput = findCamundaExtensionElement(element, "inputOutput");
		if (inputOutput != null)
		{
		  addError("camunda:inputOutput mapping unsupported for element type '" + element.TagName + "'.", element);
		}
	  }

	  protected internal virtual ParameterValueProvider createParameterValueProvider(object value, ExpressionManager expressionManager)
	  {
		if (value == null)
		{
		  return new NullValueProvider();

		}
		else if (value is string)
		{
		  Expression expression = expressionManager.createExpression((string) value);
		  return new ElValueProvider(expression);

		}
		else
		{
		  return new ConstantValueProvider(value);
		}
	  }

	  protected internal virtual void addTimeCycleWarning(Element timeCycleElement, string type)
	  {
		string warning = "It is not recommended to use a " + type + " timer event with a time cycle.";
		addWarning(warning, timeCycleElement);
	  }

	  protected internal virtual void ensureNoExpressionInMessageStartEvent(Element element, EventSubscriptionDeclaration messageStartEventSubscriptionDeclaration)
	  {
		bool eventNameContainsExpression = false;
		if (messageStartEventSubscriptionDeclaration.hasEventName())
		{
		  eventNameContainsExpression = !messageStartEventSubscriptionDeclaration.EventNameLiteralText;
		}
		if (eventNameContainsExpression)
		{
		  string messageStartName = messageStartEventSubscriptionDeclaration.UnresolvedEventName;
		  addError("Invalid message name '" + messageStartName + "' for element '" + element.TagName + "': expressions in the message start event name are not allowed!", element);
		}
	  }

	}

}