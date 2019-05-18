using System;

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
namespace org.camunda.bpm.model.bpmn.impl
{
	/// <summary>
	/// Constants used in the BPMN 2.0 Language (DI + Semantic)
	/// 
	/// @author Daniel Meyer
	/// @author Falko Menge
	/// 
	/// </summary>
	public sealed class BpmnModelConstants
	{

	  /// <summary>
	  /// The XSI namespace </summary>
	  public const string XSI_NS = "http://www.w3.org/2001/XMLSchema-instance";

	  /// <summary>
	  /// The BPMN 2.0 namespace </summary>
	  public const string BPMN20_NS = "http://www.omg.org/spec/BPMN/20100524/MODEL";

	  /// <summary>
	  /// The BPMNDI namespace </summary>
	  public const string BPMNDI_NS = "http://www.omg.org/spec/BPMN/20100524/DI";

	  /// <summary>
	  /// The DC namespace </summary>
	  public const string DC_NS = "http://www.omg.org/spec/DD/20100524/DC";

	  /// <summary>
	  /// The DI namespace </summary>
	  public const string DI_NS = "http://www.omg.org/spec/DD/20100524/DI";

	  /// <summary>
	  /// The location of the BPMN 2.0 XML schema. </summary>
	  public const string BPMN_20_SCHEMA_LOCATION = "BPMN20.xsd";

	  /// <summary>
	  /// Xml Schema is the default type language </summary>
	  public const string XML_SCHEMA_NS = "http://www.w3.org/2001/XMLSchema";

	  public const string XPATH_NS = "http://www.w3.org/1999/XPath";

	  /// @deprecated use <seealso cref="#CAMUNDA_NS"/> 
	  [Obsolete("use <seealso cref="#CAMUNDA_NS"/>")]
	  public const string ACTIVITI_NS = "http://activiti.org/bpmn";

	  /// <summary>
	  /// CAMUNDA_NS namespace </summary>
	  public const string CAMUNDA_NS = "http://camunda.org/schema/1.0/bpmn";

	  // elements ////////////////////////////////////////

	  public const string BPMN_ELEMENT_BASE_ELEMENT = "baseElement";
	  public const string BPMN_ELEMENT_DEFINITIONS = "definitions";
	  public const string BPMN_ELEMENT_DOCUMENTATION = "documentation";
	  public const string BPMN_ELEMENT_EXTENSION = "extension";
	  public const string BPMN_ELEMENT_EXTENSION_ELEMENTS = "extensionElements";
	  public const string BPMN_ELEMENT_IMPORT = "import";
	  public const string BPMN_ELEMENT_RELATIONSHIP = "relationship";
	  public const string BPMN_ELEMENT_SOURCE = "source";
	  public const string BPMN_ELEMENT_TARGET = "target";
	  public const string BPMN_ELEMENT_ROOT_ELEMENT = "rootElement";
	  public const string BPMN_ELEMENT_AUDITING = "auditing";
	  public const string BPMN_ELEMENT_MONITORING = "monitoring";
	  public const string BPMN_ELEMENT_CATEGORY_VALUE = "categoryValue";
	  public const string BPMN_ELEMENT_FLOW_ELEMENT = "flowElement";
	  public const string BPMN_ELEMENT_FLOW_NODE = "flowNode";
	  public const string BPMN_ELEMENT_CATEGORY_VALUE_REF = "categoryValueRef";
	  public const string BPMN_ELEMENT_EXPRESSION = "expression";
	  public const string BPMN_ELEMENT_CONDITION_EXPRESSION = "conditionExpression";
	  public const string BPMN_ELEMENT_SEQUENCE_FLOW = "sequenceFlow";
	  public const string BPMN_ELEMENT_INCOMING = "incoming";
	  public const string BPMN_ELEMENT_OUTGOING = "outgoing";
	  public const string BPMN_ELEMENT_DATA_STATE = "dataState";
	  public const string BPMN_ELEMENT_ITEM_DEFINITION = "itemDefinition";
	  public const string BPMN_ELEMENT_ERROR = "error";
	  public const string BPMN_ELEMENT_IN_MESSAGE_REF = "inMessageRef";
	  public const string BPMN_ELEMENT_OUT_MESSAGE_REF = "outMessageRef";
	  public const string BPMN_ELEMENT_ERROR_REF = "errorRef";
	  public const string BPMN_ELEMENT_OPERATION = "operation";
	  public const string BPMN_ELEMENT_IMPLEMENTATION_REF = "implementationRef";
	  public const string BPMN_ELEMENT_OPERATION_REF = "operationRef";
	  public const string BPMN_ELEMENT_DATA_OUTPUT = "dataOutput";
	  public const string BPMN_ELEMENT_FROM = "from";
	  public const string BPMN_ELEMENT_TO = "to";
	  public const string BPMN_ELEMENT_ASSIGNMENT = "assignment";
	  public const string BPMN_ELEMENT_ITEM_AWARE_ELEMENT = "itemAwareElement";
	  public const string BPMN_ELEMENT_DATA_OBJECT = "dataObject";
	  public const string BPMN_ELEMENT_DATA_OBJECT_REFERENCE = "dataObjectReference";
	  public const string BPMN_ELEMENT_DATA_STORE = "dataStore";
	  public const string BPMN_ELEMENT_DATA_STORE_REFERENCE = "dataStoreReference";
	  public const string BPMN_ELEMENT_DATA_INPUT = "dataInput";
	  public const string BPMN_ELEMENT_FORMAL_EXPRESSION = "formalExpression";
	  public const string BPMN_ELEMENT_DATA_ASSOCIATION = "dataAssociation";
	  public const string BPMN_ELEMENT_SOURCE_REF = "sourceRef";
	  public const string BPMN_ELEMENT_TARGET_REF = "targetRef";
	  public const string BPMN_ELEMENT_TRANSFORMATION = "transformation";
	  public const string BPMN_ELEMENT_DATA_INPUT_ASSOCIATION = "dataInputAssociation";
	  public const string BPMN_ELEMENT_DATA_OUTPUT_ASSOCIATION = "dataOutputAssociation";
	  public const string BPMN_ELEMENT_INPUT_SET = "inputSet";
	  public const string BPMN_ELEMENT_OUTPUT_SET = "outputSet";
	  public const string BPMN_ELEMENT_DATA_INPUT_REFS = "dataInputRefs";
	  public const string BPMN_ELEMENT_OPTIONAL_INPUT_REFS = "optionalInputRefs";
	  public const string BPMN_ELEMENT_WHILE_EXECUTING_INPUT_REFS = "whileExecutingInputRefs";
	  public const string BPMN_ELEMENT_OUTPUT_SET_REFS = "outputSetRefs";
	  public const string BPMN_ELEMENT_DATA_OUTPUT_REFS = "dataOutputRefs";
	  public const string BPMN_ELEMENT_OPTIONAL_OUTPUT_REFS = "optionalOutputRefs";
	  public const string BPMN_ELEMENT_WHILE_EXECUTING_OUTPUT_REFS = "whileExecutingOutputRefs";
	  public const string BPMN_ELEMENT_INPUT_SET_REFS = "inputSetRefs";
	  public const string BPMN_ELEMENT_CATCH_EVENT = "catchEvent";
	  public const string BPMN_ELEMENT_THROW_EVENT = "throwEvent";
	  public const string BPMN_ELEMENT_END_EVENT = "endEvent";
	  public const string BPMN_ELEMENT_IO_SPECIFICATION = "ioSpecification";
	  public const string BPMN_ELEMENT_LOOP_CHARACTERISTICS = "loopCharacteristics";
	  public const string BPMN_ELEMENT_RESOURCE_PARAMETER = "resourceParameter";
	  public const string BPMN_ELEMENT_RESOURCE = "resource";
	  public const string BPMN_ELEMENT_RESOURCE_PARAMETER_BINDING = "resourceParameterBinding";
	  public const string BPMN_ELEMENT_RESOURCE_ASSIGNMENT_EXPRESSION = "resourceAssignmentExpression";
	  public const string BPMN_ELEMENT_RESOURCE_ROLE = "resourceRole";
	  public const string BPMN_ELEMENT_RESOURCE_REF = "resourceRef";
	  public const string BPMN_ELEMENT_PERFORMER = "performer";
	  public const string BPMN_ELEMENT_HUMAN_PERFORMER = "humanPerformer";
	  public const string BPMN_ELEMENT_POTENTIAL_OWNER = "potentialOwner";
	  public const string BPMN_ELEMENT_ACTIVITY = "activity";
	  public const string BPMN_ELEMENT_IO_BINDING = "ioBinding";
	  public const string BPMN_ELEMENT_INTERFACE = "interface";
	  public const string BPMN_ELEMENT_EVENT = "event";
	  public const string BPMN_ELEMENT_MESSAGE = "message";
	  public const string BPMN_ELEMENT_START_EVENT = "startEvent";
	  public const string BPMN_ELEMENT_PROPERTY = "property";
	  public const string BPMN_ELEMENT_EVENT_DEFINITION = "eventDefinition";
	  public const string BPMN_ELEMENT_EVENT_DEFINITION_REF = "eventDefinitionRef";
	  public const string BPMN_ELEMENT_MESSAGE_EVENT_DEFINITION = "messageEventDefinition";
	  public const string BPMN_ELEMENT_CANCEL_EVENT_DEFINITION = "cancelEventDefinition";
	  public const string BPMN_ELEMENT_COMPENSATE_EVENT_DEFINITION = "compensateEventDefinition";
	  public const string BPMN_ELEMENT_CONDITIONAL_EVENT_DEFINITION = "conditionalEventDefinition";
	  public const string BPMN_ELEMENT_CONDITION = "condition";
	  public const string BPMN_ELEMENT_ERROR_EVENT_DEFINITION = "errorEventDefinition";
	  public const string BPMN_ELEMENT_LINK_EVENT_DEFINITION = "linkEventDefinition";
	  public const string BPMN_ELEMENT_SIGNAL_EVENT_DEFINITION = "signalEventDefinition";
	  public const string BPMN_ELEMENT_TERMINATE_EVENT_DEFINITION = "terminateEventDefinition";
	  public const string BPMN_ELEMENT_TIMER_EVENT_DEFINITION = "timerEventDefinition";
	  public const string BPMN_ELEMENT_SUPPORTED_INTERFACE_REF = "supportedInterfaceRef";
	  public const string BPMN_ELEMENT_CALLABLE_ELEMENT = "callableElement";
	  public const string BPMN_ELEMENT_PARTITION_ELEMENT = "partitionElement";
	  public const string BPMN_ELEMENT_FLOW_NODE_REF = "flowNodeRef";
	  public const string BPMN_ELEMENT_CHILD_LANE_SET = "childLaneSet";
	  public const string BPMN_ELEMENT_LANE_SET = "laneSet";
	  public const string BPMN_ELEMENT_LANE = "lane";
	  public const string BPMN_ELEMENT_ARTIFACT = "artifact";
	  public const string BPMN_ELEMENT_CORRELATION_PROPERTY_RETRIEVAL_EXPRESSION = "correlationPropertyRetrievalExpression";
	  public const string BPMN_ELEMENT_MESSAGE_PATH = "messagePath";
	  public const string BPMN_ELEMENT_DATA_PATH = "dataPath";
	  public const string BPMN_ELEMENT_CALL_ACTIVITY = "callActivity";
	  public const string BPMN_ELEMENT_CORRELATION_PROPERTY_BINDING = "correlationPropertyBinding";
	  public const string BPMN_ELEMENT_CORRELATION_PROPERTY = "correlationProperty";
	  public const string BPMN_ELEMENT_CORRELATION_PROPERTY_REF = "correlationPropertyRef";
	  public const string BPMN_ELEMENT_CORRELATION_KEY = "correlationKey";
	  public const string BPMN_ELEMENT_CORRELATION_SUBSCRIPTION = "correlationSubscription";
	  public const string BPMN_ELEMENT_SUPPORTS = "supports";
	  public const string BPMN_ELEMENT_PROCESS = "process";
	  public const string BPMN_ELEMENT_TASK = "task";
	  public const string BPMN_ELEMENT_SEND_TASK = "sendTask";
	  public const string BPMN_ELEMENT_SERVICE_TASK = "serviceTask";
	  public const string BPMN_ELEMENT_SCRIPT_TASK = "scriptTask";
	  public const string BPMN_ELEMENT_USER_TASK = "userTask";
	  public const string BPMN_ELEMENT_RECEIVE_TASK = "receiveTask";
	  public const string BPMN_ELEMENT_BUSINESS_RULE_TASK = "businessRuleTask";
	  public const string BPMN_ELEMENT_MANUAL_TASK = "manualTask";
	  public const string BPMN_ELEMENT_SCRIPT = "script";
	  public const string BPMN_ELEMENT_RENDERING = "rendering";
	  public const string BPMN_ELEMENT_BOUNDARY_EVENT = "boundaryEvent";
	  public const string BPMN_ELEMENT_SUB_PROCESS = "subProcess";
	  public const string BPMN_ELEMENT_TRANSACTION = "transaction";
	  public const string BPMN_ELEMENT_GATEWAY = "gateway";
	  public const string BPMN_ELEMENT_PARALLEL_GATEWAY = "parallelGateway";
	  public const string BPMN_ELEMENT_EXCLUSIVE_GATEWAY = "exclusiveGateway";
	  public const string BPMN_ELEMENT_INTERMEDIATE_CATCH_EVENT = "intermediateCatchEvent";
	  public const string BPMN_ELEMENT_INTERMEDIATE_THROW_EVENT = "intermediateThrowEvent";
	  public const string BPMN_ELEMENT_END_POINT = "endPoint";
	  public const string BPMN_ELEMENT_PARTICIPANT_MULTIPLICITY = "participantMultiplicity";
	  public const string BPMN_ELEMENT_PARTICIPANT = "participant";
	  public const string BPMN_ELEMENT_PARTICIPANT_REF = "participantRef";
	  public const string BPMN_ELEMENT_INTERFACE_REF = "interfaceRef";
	  public const string BPMN_ELEMENT_END_POINT_REF = "endPointRef";
	  public const string BPMN_ELEMENT_MESSAGE_FLOW = "messageFlow";
	  public const string BPMN_ELEMENT_MESSAGE_FLOW_REF = "messageFlowRef";
	  public const string BPMN_ELEMENT_CONVERSATION_NODE = "conversationNode";
	  public const string BPMN_ELEMENT_CONVERSATION = "conversation";
	  public const string BPMN_ELEMENT_SUB_CONVERSATION = "subConversation";
	  public const string BPMN_ELEMENT_GLOBAL_CONVERSATION = "globalConversation";
	  public const string BPMN_ELEMENT_CALL_CONVERSATION = "callConversation";
	  public const string BPMN_ELEMENT_PARTICIPANT_ASSOCIATION = "participantAssociation";
	  public const string BPMN_ELEMENT_INNER_PARTICIPANT_REF = "innerParticipantRef";
	  public const string BPMN_ELEMENT_OUTER_PARTICIPANT_REF = "outerParticipantRef";
	  public const string BPMN_ELEMENT_CONVERSATION_ASSOCIATION = "conversationAssociation";
	  public const string BPMN_ELEMENT_MESSAGE_FLOW_ASSOCIATION = "messageFlowAssociation";
	  public const string BPMN_ELEMENT_CONVERSATION_LINK = "conversationLink";
	  public const string BPMN_ELEMENT_COLLABORATION = "collaboration";
	  public const string BPMN_ELEMENT_ASSOCIATION = "association";
	  public const string BPMN_ELEMENT_SIGNAL = "signal";
	  public const string BPMN_ELEMENT_TIME_DATE = "timeDate";
	  public const string BPMN_ELEMENT_TIME_DURATION = "timeDuration";
	  public const string BPMN_ELEMENT_TIME_CYCLE = "timeCycle";
	  public const string BPMN_ELEMENT_ESCALATION = "escalation";
	  public const string BPMN_ELEMENT_ESCALATION_EVENT_DEFINITION = "escalationEventDefinition";
	  public const string BPMN_ELEMENT_ACTIVATION_CONDITION = "activationCondition";
	  public const string BPMN_ELEMENT_COMPLEX_GATEWAY = "complexGateway";
	  public const string BPMN_ELEMENT_EVENT_BASED_GATEWAY = "eventBasedGateway";
	  public const string BPMN_ELEMENT_INCLUSIVE_GATEWAY = "inclusiveGateway";
	  public const string BPMN_ELEMENT_TEXT_ANNOTATION = "textAnnotation";
	  public const string BPMN_ELEMENT_TEXT = "text";
	  public const string BPMN_ELEMENT_COMPLEX_BEHAVIOR_DEFINITION = "complexBehaviorDefinition";
	  public const string BPMN_ELEMENT_MULTI_INSTANCE_LOOP_CHARACTERISTICS = "multiInstanceLoopCharacteristics";
	  public const string BPMN_ELEMENT_LOOP_CARDINALITY = "loopCardinality";
	  public const string BPMN_ELEMENT_COMPLETION_CONDITION = "completionCondition";
	  public const string BPMN_ELEMENT_OUTPUT_DATA_ITEM = "outputDataItem";
	  public const string BPMN_ELEMENT_INPUT_DATA_ITEM = "inputDataItem";
	  public const string BPMN_ELEMENT_LOOP_DATA_OUTPUT_REF = "loopDataOutputRef";
	  public const string BPMN_ELEMENT_LOOP_DATA_INPUT_REF = "loopDataInputRef";
	  public const string BPMN_ELEMENT_IS_SEQUENTIAL = "isSequential";
	  public const string BPMN_ELEMENT_BEHAVIOR = "behavior";
	  public const string BPMN_ELEMENT_ONE_BEHAVIOR_EVENT_REF = "oneBehaviorEventRef";
	  public const string BPMN_ELEMENT_NONE_BEHAVIOR_EVENT_REF = "noneBehaviorEventRef";

	  /// <summary>
	  /// DC </summary>

	  public const string DC_ELEMENT_FONT = "Font";
	  public const string DC_ELEMENT_POINT = "Point";
	  public const string DC_ELEMENT_BOUNDS = "Bounds";

	  /// <summary>
	  /// DI </summary>

	  public const string DI_ELEMENT_DIAGRAM_ELEMENT = "DiagramElement";
	  public const string DI_ELEMENT_DIAGRAM = "Diagram";
	  public const string DI_ELEMENT_EDGE = "Edge";
	  public const string DI_ELEMENT_EXTENSION = "extension";
	  public const string DI_ELEMENT_LABELED_EDGE = "LabeledEdge";
	  public const string DI_ELEMENT_LABEL = "Label";
	  public const string DI_ELEMENT_LABELED_SHAPE = "LabeledShape";
	  public const string DI_ELEMENT_NODE = "Node";
	  public const string DI_ELEMENT_PLANE = "Plane";
	  public const string DI_ELEMENT_SHAPE = "Shape";
	  public const string DI_ELEMENT_STYLE = "Style";
	  public const string DI_ELEMENT_WAYPOINT = "waypoint";

	  /// <summary>
	  /// BPMNDI </summary>

	  public const string BPMNDI_ELEMENT_BPMN_DIAGRAM = "BPMNDiagram";
	  public const string BPMNDI_ELEMENT_BPMN_PLANE = "BPMNPlane";
	  public const string BPMNDI_ELEMENT_BPMN_LABEL_STYLE = "BPMNLabelStyle";
	  public const string BPMNDI_ELEMENT_BPMN_SHAPE = "BPMNShape";
	  public const string BPMNDI_ELEMENT_BPMN_LABEL = "BPMNLabel";
	  public const string BPMNDI_ELEMENT_BPMN_EDGE = "BPMNEdge";

	  /// <summary>
	  /// camunda extensions </summary>

	  public const string CAMUNDA_ELEMENT_CONNECTOR = "connector";
	  public const string CAMUNDA_ELEMENT_CONNECTOR_ID = "connectorId";
	  public const string CAMUNDA_ELEMENT_CONSTRAINT = "constraint";
	  public const string CAMUNDA_ELEMENT_ENTRY = "entry";
	  public const string CAMUNDA_ELEMENT_EXECUTION_LISTENER = "executionListener";
	  public const string CAMUNDA_ELEMENT_EXPRESSION = "expression";
	  public const string CAMUNDA_ELEMENT_FAILED_JOB_RETRY_TIME_CYCLE = "failedJobRetryTimeCycle";
	  public const string CAMUNDA_ELEMENT_FIELD = "field";
	  public const string CAMUNDA_ELEMENT_FORM_DATA = "formData";
	  public const string CAMUNDA_ELEMENT_FORM_FIELD = "formField";
	  public const string CAMUNDA_ELEMENT_FORM_PROPERTY = "formProperty";
	  public const string CAMUNDA_ELEMENT_IN = "in";
	  public const string CAMUNDA_ELEMENT_INPUT_OUTPUT = "inputOutput";
	  public const string CAMUNDA_ELEMENT_INPUT_PARAMETER = "inputParameter";
	  public const string CAMUNDA_ELEMENT_LIST = "list";
	  public const string CAMUNDA_ELEMENT_MAP = "map";
	  public const string CAMUNDA_ELEMENT_OUTPUT_PARAMETER = "outputParameter";
	  public const string CAMUNDA_ELEMENT_OUT = "out";
	  public const string CAMUNDA_ELEMENT_POTENTIAL_STARTER = "potentialStarter";
	  public const string CAMUNDA_ELEMENT_PROPERTIES = "properties";
	  public const string CAMUNDA_ELEMENT_PROPERTY = "property";
	  public const string CAMUNDA_ELEMENT_SCRIPT = "script";
	  public const string CAMUNDA_ELEMENT_STRING = "string";
	  public const string CAMUNDA_ELEMENT_TASK_LISTENER = "taskListener";
	  public const string CAMUNDA_ELEMENT_VALIDATION = "validation";
	  public const string CAMUNDA_ELEMENT_VALUE = "value";

	  // attributes //////////////////////////////////////

	  /// <summary>
	  /// XSI attributes * </summary>

	  public const string XSI_ATTRIBUTE_TYPE = "type";

	  /// <summary>
	  /// BPMN attributes * </summary>

	  public const string BPMN_ATTRIBUTE_EXPORTER = "exporter";
	  public const string BPMN_ATTRIBUTE_EXPORTER_VERSION = "exporterVersion";
	  public const string BPMN_ATTRIBUTE_EXPRESSION_LANGUAGE = "expressionLanguage";
	  public const string BPMN_ATTRIBUTE_ID = "id";
	  public const string BPMN_ATTRIBUTE_NAME = "name";
	  public const string BPMN_ATTRIBUTE_TARGET_NAMESPACE = "targetNamespace";
	  public const string BPMN_ATTRIBUTE_TYPE_LANGUAGE = "typeLanguage";
	  public const string BPMN_ATTRIBUTE_NAMESPACE = "namespace";
	  public const string BPMN_ATTRIBUTE_LOCATION = "location";
	  public const string BPMN_ATTRIBUTE_IMPORT_TYPE = "importType";
	  public const string BPMN_ATTRIBUTE_TEXT_FORMAT = "textFormat";
	  public const string BPMN_ATTRIBUTE_PROCESS_TYPE = "processType";
	  public const string BPMN_ATTRIBUTE_IS_CLOSED = "isClosed";
	  public const string BPMN_ATTRIBUTE_IS_EXECUTABLE = "isExecutable";
	  public const string BPMN_ATTRIBUTE_MESSAGE_REF = "messageRef";
	  public const string BPMN_ATTRIBUTE_DEFINITION = "definition";
	  public const string BPMN_ATTRIBUTE_MUST_UNDERSTAND = "mustUnderstand";
	  public const string BPMN_ATTRIBUTE_TYPE = "type";
	  public const string BPMN_ATTRIBUTE_DIRECTION = "direction";
	  public const string BPMN_ATTRIBUTE_SOURCE_REF = "sourceRef";
	  public const string BPMN_ATTRIBUTE_TARGET_REF = "targetRef";
	  public const string BPMN_ATTRIBUTE_IS_IMMEDIATE = "isImmediate";
	  public const string BPMN_ATTRIBUTE_VALUE = "value";
	  public const string BPMN_ATTRIBUTE_STRUCTURE_REF = "structureRef";
	  public const string BPMN_ATTRIBUTE_IS_COLLECTION = "isCollection";
	  public const string BPMN_ATTRIBUTE_ITEM_KIND = "itemKind";
	  public const string BPMN_ATTRIBUTE_ITEM_REF = "itemRef";
	  public const string BPMN_ATTRIBUTE_ITEM_SUBJECT_REF = "itemSubjectRef";
	  public const string BPMN_ATTRIBUTE_ERROR_CODE = "errorCode";
	  public const string BPMN_ATTRIBUTE_LANGUAGE = "language";
	  public const string BPMN_ATTRIBUTE_EVALUATES_TO_TYPE_REF = "evaluatesToTypeRef";
	  public const string BPMN_ATTRIBUTE_PARALLEL_MULTIPLE = "parallelMultiple";
	  public const string BPMN_ATTRIBUTE_IS_INTERRUPTING = "isInterrupting";
	  public const string BPMN_ATTRIBUTE_IS_REQUIRED = "isRequired";
	  public const string BPMN_ATTRIBUTE_PARAMETER_REF = "parameterRef";
	  public const string BPMN_ATTRIBUTE_IS_FOR_COMPENSATION = "isForCompensation";
	  public const string BPMN_ATTRIBUTE_START_QUANTITY = "startQuantity";
	  public const string BPMN_ATTRIBUTE_COMPLETION_QUANTITY = "completionQuantity";
	  public const string BPMN_ATTRIBUTE_DEFAULT = "default";
	  public const string BPMN_ATTRIBUTE_OPERATION_REF = "operationRef";
	  public const string BPMN_ATTRIBUTE_INPUT_DATA_REF = "inputDataRef";
	  public const string BPMN_ATTRIBUTE_OUTPUT_DATA_REF = "outputDataRef";
	  public const string BPMN_ATTRIBUTE_IMPLEMENTATION_REF = "implementationRef";
	  public const string BPMN_ATTRIBUTE_PARTITION_ELEMENT_REF = "partitionElementRef";
	  public const string BPMN_ATTRIBUTE_CORRELATION_PROPERTY_REF = "correlationPropertyRef";
	  public const string BPMN_ATTRIBUTE_CORRELATION_KEY_REF = "correlationKeyRef";
	  public const string BPMN_ATTRIBUTE_IMPLEMENTATION = "implementation";
	  public const string BPMN_ATTRIBUTE_SCRIPT_FORMAT = "scriptFormat";
	  public const string BPMN_ATTRIBUTE_INSTANTIATE = "instantiate";
	  public const string BPMN_ATTRIBUTE_CANCEL_ACTIVITY = "cancelActivity";
	  public const string BPMN_ATTRIBUTE_ATTACHED_TO_REF = "attachedToRef";
	  public const string BPMN_ATTRIBUTE_TRIGGERED_BY_EVENT = "triggeredByEvent";
	  public const string BPMN_ATTRIBUTE_GATEWAY_DIRECTION = "gatewayDirection";
	  public const string BPMN_ATTRIBUTE_CALLED_ELEMENT = "calledElement";
	  public const string BPMN_ATTRIBUTE_MINIMUM = "minimum";
	  public const string BPMN_ATTRIBUTE_MAXIMUM = "maximum";
	  public const string BPMN_ATTRIBUTE_PROCESS_REF = "processRef";
	  public const string BPMN_ATTRIBUTE_CALLED_COLLABORATION_REF = "calledCollaborationRef";
	  public const string BPMN_ATTRIBUTE_INNER_CONVERSATION_NODE_REF = "innerConversationNodeRef";
	  public const string BPMN_ATTRIBUTE_OUTER_CONVERSATION_NODE_REF = "outerConversationNodeRef";
	  public const string BPMN_ATTRIBUTE_INNER_MESSAGE_FLOW_REF = "innerMessageFlowRef";
	  public const string BPMN_ATTRIBUTE_OUTER_MESSAGE_FLOW_REF = "outerMessageFlowRef";
	  public const string BPMN_ATTRIBUTE_ASSOCIATION_DIRECTION = "associationDirection";
	  public const string BPMN_ATTRIBUTE_WAIT_FOR_COMPLETION = "waitForCompletion";
	  public const string BPMN_ATTRIBUTE_ACTIVITY_REF = "activityRef";
	  public const string BPMN_ATTRIBUTE_ERROR_REF = "errorRef";
	  public const string BPMN_ATTRIBUTE_SIGNAL_REF = "signalRef";
	  public const string BPMN_ATTRIBUTE_ESCALATION_CODE = "escalationCode";
	  public const string BPMN_ATTRIBUTE_ESCALATION_REF = "escalationRef";
	  public const string BPMN_ATTRIBUTE_EVENT_GATEWAY_TYPE = "eventGatewayType";
	  public const string BPMN_ATTRIBUTE_DATA_OBJECT_REF = "dataObjectRef";
	  public const string BPMN_ATTRIBUTE_DATA_STORE_REF = "dataStoreRef";
	  public const string BPMN_ATTRIBUTE_METHOD = "method";
	  public const string BPMN_ATTRIBUTE_CAPACITY = "capacity";
	  public const string BPMN_ATTRIBUTE_IS_UNLIMITED = "isUnlimited";

	  /// <summary>
	  /// DC </summary>

	  public const string DC_ATTRIBUTE_NAME = "name";
	  public const string DC_ATTRIBUTE_SIZE = "size";
	  public const string DC_ATTRIBUTE_IS_BOLD = "isBold";
	  public const string DC_ATTRIBUTE_IS_ITALIC = "isItalic";
	  public const string DC_ATTRIBUTE_IS_UNDERLINE = "isUnderline";
	  public const string DC_ATTRIBUTE_IS_STRIKE_THROUGH = "isStrikeThrough";
	  public const string DC_ATTRIBUTE_X = "x";
	  public const string DC_ATTRIBUTE_Y = "y";
	  public const string DC_ATTRIBUTE_WIDTH = "width";
	  public const string DC_ATTRIBUTE_HEIGHT = "height";

	  /// <summary>
	  /// DI </summary>

	  public const string DI_ATTRIBUTE_ID = "id";
	  public const string DI_ATTRIBUTE_NAME = "name";
	  public const string DI_ATTRIBUTE_DOCUMENTATION = "documentation";
	  public const string DI_ATTRIBUTE_RESOLUTION = "resolution";

	  /// <summary>
	  /// BPMNDI </summary>

	  public const string BPMNDI_ATTRIBUTE_BPMN_ELEMENT = "bpmnElement";
	  public const string BPMNDI_ATTRIBUTE_SOURCE_ELEMENT = "sourceElement";
	  public const string BPMNDI_ATTRIBUTE_TARGET_ELEMENT = "targetElement";
	  public const string BPMNDI_ATTRIBUTE_MESSAGE_VISIBLE_KIND = "messageVisibleKind";
	  public const string BPMNDI_ATTRIBUTE_IS_HORIZONTAL = "isHorizontal";
	  public const string BPMNDI_ATTRIBUTE_IS_EXPANDED = "isExpanded";
	  public const string BPMNDI_ATTRIBUTE_IS_MARKER_VISIBLE = "isMarkerVisible";
	  public const string BPMNDI_ATTRIBUTE_IS_MESSAGE_VISIBLE = "isMessageVisible";
	  public const string BPMNDI_ATTRIBUTE_PARTICIPANT_BAND_KIND = "participantBandKind";
	  public const string BPMNDI_ATTRIBUTE_CHOREOGRAPHY_ACTIVITY_SHAPE = "choreographyActivityShape";
	  public const string BPMNDI_ATTRIBUTE_LABEL_STYLE = "labelStyle";

	  /// <summary>
	  /// camunda extensions </summary>

	  public const string CAMUNDA_ATTRIBUTE_ASSIGNEE = "assignee";
	  public const string CAMUNDA_ATTRIBUTE_ASYNC = "async";
	  public const string CAMUNDA_ATTRIBUTE_ASYNC_BEFORE = "asyncBefore";
	  public const string CAMUNDA_ATTRIBUTE_ASYNC_AFTER = "asyncAfter";
	  public const string CAMUNDA_ATTRIBUTE_BUSINESS_KEY = "businessKey";
	  public const string CAMUNDA_ATTRIBUTE_CALLED_ELEMENT_BINDING = "calledElementBinding";
	  public const string CAMUNDA_ATTRIBUTE_CALLED_ELEMENT_VERSION = "calledElementVersion";
	  public const string CAMUNDA_ATTRIBUTE_CALLED_ELEMENT_VERSION_TAG = "calledElementVersionTag";
	  public const string CAMUNDA_ATTRIBUTE_CALLED_ELEMENT_TENANT_ID = "calledElementTenantId";
	  public const string CAMUNDA_ATTRIBUTE_CANDIDATE_GROUPS = "candidateGroups";
	  public const string CAMUNDA_ATTRIBUTE_CANDIDATE_STARTER_GROUPS = "candidateStarterGroups";
	  public const string CAMUNDA_ATTRIBUTE_CANDIDATE_STARTER_USERS = "candidateStarterUsers";
	  public const string CAMUNDA_ATTRIBUTE_CANDIDATE_USERS = "candidateUsers";
	  public const string CAMUNDA_ATTRIBUTE_CLASS = "class";
	  public const string CAMUNDA_ATTRIBUTE_COLLECTION = "collection";
	  public const string CAMUNDA_ATTRIBUTE_CONFIG = "config";
	  public const string CAMUNDA_ATTRIBUTE_DATE_PATTERN = "datePattern";
	  public const string CAMUNDA_ATTRIBUTE_DECISION_REF = "decisionRef";
	  public const string CAMUNDA_ATTRIBUTE_DECISION_REF_BINDING = "decisionRefBinding";
	  public const string CAMUNDA_ATTRIBUTE_DECISION_REF_VERSION = "decisionRefVersion";
	  public const string CAMUNDA_ATTRIBUTE_DECISION_REF_VERSION_TAG = "decisionRefVersionTag";
	  public const string CAMUNDA_ATTRIBUTE_DECISION_REF_TENANT_ID = "decisionRefTenantId";
	  public const string CAMUNDA_ATTRIBUTE_DEFAULT = "default";
	  public const string CAMUNDA_ATTRIBUTE_DEFAULT_VALUE = "defaultValue";
	  public const string CAMUNDA_ATTRIBUTE_DELEGATE_EXPRESSION = "delegateExpression";
	  public const string CAMUNDA_ATTRIBUTE_DUE_DATE = "dueDate";
	  public const string CAMUNDA_ATTRIBUTE_FOLLOW_UP_DATE = "followUpDate";
	  public const string CAMUNDA_ATTRIBUTE_ELEMENT_VARIABLE = "elementVariable";
	  public const string CAMUNDA_ATTRIBUTE_EVENT = "event";
	  public const string CAMUNDA_ATTRIBUTE_ERROR_CODE_VARIABLE = "errorCodeVariable";
	  public const string CAMUNDA_ATTRIBUTE_ERROR_MESSAGE_VARIABLE = "errorMessageVariable";
	  public const string CAMUNDA_ATTRIBUTE_EXCLUSIVE = "exclusive";
	  public const string CAMUNDA_ATTRIBUTE_EXPRESSION = "expression";
	  public const string CAMUNDA_ATTRIBUTE_FORM_HANDLER_CLASS = "formHandlerClass";
	  public const string CAMUNDA_ATTRIBUTE_FORM_KEY = "formKey";
	  public const string CAMUNDA_ATTRIBUTE_ID = "id";
	  public const string CAMUNDA_ATTRIBUTE_INITIATOR = "initiator";
	  public const string CAMUNDA_ATTRIBUTE_JOB_PRIORITY = "jobPriority";
	  public const string CAMUNDA_ATTRIBUTE_TASK_PRIORITY = "taskPriority";
	  public const string CAMUNDA_ATTRIBUTE_KEY = "key";
	  public const string CAMUNDA_ATTRIBUTE_LABEL = "label";
	  public const string CAMUNDA_ATTRIBUTE_LOCAL = "local";
	  public const string CAMUNDA_ATTRIBUTE_MAP_DECISION_RESULT = "mapDecisionResult";
	  public const string CAMUNDA_ATTRIBUTE_NAME = "name";
	  public const string CAMUNDA_ATTRIBUTE_PRIORITY = "priority";
	  public const string CAMUNDA_ATTRIBUTE_READABLE = "readable";
	  public const string CAMUNDA_ATTRIBUTE_REQUIRED = "required";
	  public const string CAMUNDA_ATTRIBUTE_RESOURCE = "resource";
	  public const string CAMUNDA_ATTRIBUTE_RESULT_VARIABLE = "resultVariable";
	  public const string CAMUNDA_ATTRIBUTE_SCRIPT_FORMAT = "scriptFormat";
	  public const string CAMUNDA_ATTRIBUTE_SOURCE = "source";
	  public const string CAMUNDA_ATTRIBUTE_SOURCE_EXPRESSION = "sourceExpression";
	  public const string CAMUNDA_ATTRIBUTE_STRING_VALUE = "stringValue";
	  public const string CAMUNDA_ATTRIBUTE_TARGET = "target";
	  public const string CAMUNDA_ATTRIBUTE_TOPIC = "topic";
	  public const string CAMUNDA_ATTRIBUTE_TYPE = "type";
	  public const string CAMUNDA_ATTRIBUTE_VALUE = "value";
	  public const string CAMUNDA_ATTRIBUTE_VARIABLE = "variable";
	  public const string CAMUNDA_ATTRIBUTE_VARIABLE_MAPPING_CLASS = "variableMappingClass";
	  public const string CAMUNDA_ATTRIBUTE_VARIABLE_MAPPING_DELEGATE_EXPRESSION = "variableMappingDelegateExpression";
	  public const string CAMUNDA_ATTRIBUTE_VARIABLES = "variables";
	  public const string CAMUNDA_ATTRIBUTE_WRITEABLE = "writeable";
	  public const string CAMUNDA_ATTRIBUTE_CASE_REF = "caseRef";
	  public const string CAMUNDA_ATTRIBUTE_CASE_BINDING = "caseBinding";
	  public const string CAMUNDA_ATTRIBUTE_CASE_VERSION = "caseVersion";
	  public const string CAMUNDA_ATTRIBUTE_CASE_TENANT_ID = "caseTenantId";
	  public const string CAMUNDA_ATTRIBUTE_VARIABLE_NAME = "variableName";
	  public const string CAMUNDA_ATTRIBUTE_VARIABLE_EVENTS = "variableEvents";
	  public const string CAMUNDA_ATTRIBUTE_HISTORY_TIME_TO_LIVE = "historyTimeToLive";
	  public const string CAMUNDA_ATTRIBUTE_IS_STARTABLE_IN_TASKLIST = "isStartableInTasklist";
	  public const string CAMUNDA_ATTRIBUTE_VERSION_TAG = "versionTag";
	}

}