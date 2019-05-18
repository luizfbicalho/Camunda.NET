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
namespace org.camunda.bpm.model.bpmn
{
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.model.bpmn.impl.BpmnModelConstants.ACTIVITI_NS;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.model.bpmn.impl.BpmnModelConstants.BPMN20_NS;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.model.bpmn.impl.BpmnModelConstants.CAMUNDA_NS;


	using ProcessBuilder = org.camunda.bpm.model.bpmn.builder.ProcessBuilder;
	using BpmnParser = org.camunda.bpm.model.bpmn.impl.BpmnParser;
	using org.camunda.bpm.model.bpmn.impl.instance;
	using BpmnDiagramImpl = org.camunda.bpm.model.bpmn.impl.instance.bpmndi.BpmnDiagramImpl;
	using BpmnEdgeImpl = org.camunda.bpm.model.bpmn.impl.instance.bpmndi.BpmnEdgeImpl;
	using BpmnLabelImpl = org.camunda.bpm.model.bpmn.impl.instance.bpmndi.BpmnLabelImpl;
	using BpmnLabelStyleImpl = org.camunda.bpm.model.bpmn.impl.instance.bpmndi.BpmnLabelStyleImpl;
	using BpmnPlaneImpl = org.camunda.bpm.model.bpmn.impl.instance.bpmndi.BpmnPlaneImpl;
	using BpmnShapeImpl = org.camunda.bpm.model.bpmn.impl.instance.bpmndi.BpmnShapeImpl;
	using CamundaConnectorIdImpl = org.camunda.bpm.model.bpmn.impl.instance.camunda.CamundaConnectorIdImpl;
	using CamundaConnectorImpl = org.camunda.bpm.model.bpmn.impl.instance.camunda.CamundaConnectorImpl;
	using CamundaConstraintImpl = org.camunda.bpm.model.bpmn.impl.instance.camunda.CamundaConstraintImpl;
	using CamundaEntryImpl = org.camunda.bpm.model.bpmn.impl.instance.camunda.CamundaEntryImpl;
	using CamundaExecutionListenerImpl = org.camunda.bpm.model.bpmn.impl.instance.camunda.CamundaExecutionListenerImpl;
	using CamundaExpressionImpl = org.camunda.bpm.model.bpmn.impl.instance.camunda.CamundaExpressionImpl;
	using CamundaFailedJobRetryTimeCycleImpl = org.camunda.bpm.model.bpmn.impl.instance.camunda.CamundaFailedJobRetryTimeCycleImpl;
	using CamundaFieldImpl = org.camunda.bpm.model.bpmn.impl.instance.camunda.CamundaFieldImpl;
	using CamundaFormDataImpl = org.camunda.bpm.model.bpmn.impl.instance.camunda.CamundaFormDataImpl;
	using CamundaFormFieldImpl = org.camunda.bpm.model.bpmn.impl.instance.camunda.CamundaFormFieldImpl;
	using CamundaFormPropertyImpl = org.camunda.bpm.model.bpmn.impl.instance.camunda.CamundaFormPropertyImpl;
	using CamundaInImpl = org.camunda.bpm.model.bpmn.impl.instance.camunda.CamundaInImpl;
	using CamundaInputOutputImpl = org.camunda.bpm.model.bpmn.impl.instance.camunda.CamundaInputOutputImpl;
	using CamundaInputParameterImpl = org.camunda.bpm.model.bpmn.impl.instance.camunda.CamundaInputParameterImpl;
	using CamundaListImpl = org.camunda.bpm.model.bpmn.impl.instance.camunda.CamundaListImpl;
	using CamundaMapImpl = org.camunda.bpm.model.bpmn.impl.instance.camunda.CamundaMapImpl;
	using CamundaOutImpl = org.camunda.bpm.model.bpmn.impl.instance.camunda.CamundaOutImpl;
	using CamundaOutputParameterImpl = org.camunda.bpm.model.bpmn.impl.instance.camunda.CamundaOutputParameterImpl;
	using CamundaPotentialStarterImpl = org.camunda.bpm.model.bpmn.impl.instance.camunda.CamundaPotentialStarterImpl;
	using CamundaPropertiesImpl = org.camunda.bpm.model.bpmn.impl.instance.camunda.CamundaPropertiesImpl;
	using CamundaPropertyImpl = org.camunda.bpm.model.bpmn.impl.instance.camunda.CamundaPropertyImpl;
	using CamundaScriptImpl = org.camunda.bpm.model.bpmn.impl.instance.camunda.CamundaScriptImpl;
	using CamundaStringImpl = org.camunda.bpm.model.bpmn.impl.instance.camunda.CamundaStringImpl;
	using CamundaTaskListenerImpl = org.camunda.bpm.model.bpmn.impl.instance.camunda.CamundaTaskListenerImpl;
	using CamundaValidationImpl = org.camunda.bpm.model.bpmn.impl.instance.camunda.CamundaValidationImpl;
	using CamundaValueImpl = org.camunda.bpm.model.bpmn.impl.instance.camunda.CamundaValueImpl;
	using BoundsImpl = org.camunda.bpm.model.bpmn.impl.instance.dc.BoundsImpl;
	using FontImpl = org.camunda.bpm.model.bpmn.impl.instance.dc.FontImpl;
	using PointImpl = org.camunda.bpm.model.bpmn.impl.instance.dc.PointImpl;
	using DiagramElementImpl = org.camunda.bpm.model.bpmn.impl.instance.di.DiagramElementImpl;
	using DiagramImpl = org.camunda.bpm.model.bpmn.impl.instance.di.DiagramImpl;
	using EdgeImpl = org.camunda.bpm.model.bpmn.impl.instance.di.EdgeImpl;
	using LabelImpl = org.camunda.bpm.model.bpmn.impl.instance.di.LabelImpl;
	using LabeledEdgeImpl = org.camunda.bpm.model.bpmn.impl.instance.di.LabeledEdgeImpl;
	using LabeledShapeImpl = org.camunda.bpm.model.bpmn.impl.instance.di.LabeledShapeImpl;
	using NodeImpl = org.camunda.bpm.model.bpmn.impl.instance.di.NodeImpl;
	using PlaneImpl = org.camunda.bpm.model.bpmn.impl.instance.di.PlaneImpl;
	using ShapeImpl = org.camunda.bpm.model.bpmn.impl.instance.di.ShapeImpl;
	using StyleImpl = org.camunda.bpm.model.bpmn.impl.instance.di.StyleImpl;
	using WaypointImpl = org.camunda.bpm.model.bpmn.impl.instance.di.WaypointImpl;
	using Definitions = org.camunda.bpm.model.bpmn.instance.Definitions;
	using Process = org.camunda.bpm.model.bpmn.instance.Process;
	using BpmnDiagram = org.camunda.bpm.model.bpmn.instance.bpmndi.BpmnDiagram;
	using BpmnPlane = org.camunda.bpm.model.bpmn.instance.bpmndi.BpmnPlane;
	using Model = org.camunda.bpm.model.xml.Model;
	using ModelBuilder = org.camunda.bpm.model.xml.ModelBuilder;
	using ModelException = org.camunda.bpm.model.xml.ModelException;
	using ModelParseException = org.camunda.bpm.model.xml.ModelParseException;
	using ModelValidationException = org.camunda.bpm.model.xml.ModelValidationException;
	using ModelElementInstanceImpl = org.camunda.bpm.model.xml.impl.instance.ModelElementInstanceImpl;
	using IoUtil = org.camunda.bpm.model.xml.impl.util.IoUtil;

	/// <summary>
	/// <para>Provides access to the camunda BPMN model api.</para>
	/// 
	/// @author Daniel Meyer
	/// 
	/// </summary>
	public class Bpmn
	{

	  /// <summary>
	  /// the singleton instance of <seealso cref="Bpmn"/>. If you want to customize the behavior of Bpmn,
	  /// replace this instance with an instance of a custom subclass of <seealso cref="Bpmn"/>. 
	  /// </summary>
	  public static Bpmn INSTANCE = new Bpmn();

	  /// <summary>
	  /// the parser used by the Bpmn implementation. </summary>
	  private BpmnParser bpmnParser = new BpmnParser();
	  private readonly ModelBuilder bpmnModelBuilder;

	  /// <summary>
	  /// The <seealso cref="Model"/>
	  /// </summary>
	  private Model bpmnModel;

	  /// <summary>
	  /// Allows reading a <seealso cref="BpmnModelInstance"/> from a File.
	  /// </summary>
	  /// <param name="file"> the <seealso cref="File"/> to read the <seealso cref="BpmnModelInstance"/> from </param>
	  /// <returns> the model read </returns>
	  /// <exception cref="BpmnModelException"> if the model cannot be read </exception>
	  public static BpmnModelInstance readModelFromFile(File file)
	  {
		return INSTANCE.doReadModelFromFile(file);
	  }

	  /// <summary>
	  /// Allows reading a <seealso cref="BpmnModelInstance"/> from an <seealso cref="InputStream"/>
	  /// </summary>
	  /// <param name="stream"> the <seealso cref="InputStream"/> to read the <seealso cref="BpmnModelInstance"/> from </param>
	  /// <returns> the model read </returns>
	  /// <exception cref="ModelParseException"> if the model cannot be read </exception>
	  public static BpmnModelInstance readModelFromStream(Stream stream)
	  {
		return INSTANCE.doReadModelFromInputStream(stream);
	  }

	  /// <summary>
	  /// Allows writing a <seealso cref="BpmnModelInstance"/> to a File. It will be
	  /// validated before writing.
	  /// </summary>
	  /// <param name="file"> the <seealso cref="File"/> to write the <seealso cref="BpmnModelInstance"/> to </param>
	  /// <param name="modelInstance"> the <seealso cref="BpmnModelInstance"/> to write </param>
	  /// <exception cref="BpmnModelException"> if the model cannot be written </exception>
	  /// <exception cref="ModelValidationException"> if the model is not valid </exception>
	  public static void writeModelToFile(File file, BpmnModelInstance modelInstance)
	  {
		INSTANCE.doWriteModelToFile(file, modelInstance);
	  }

	  /// <summary>
	  /// Allows writing a <seealso cref="BpmnModelInstance"/> to an <seealso cref="OutputStream"/>. It will be
	  /// validated before writing.
	  /// </summary>
	  /// <param name="stream"> the <seealso cref="OutputStream"/> to write the <seealso cref="BpmnModelInstance"/> to </param>
	  /// <param name="modelInstance"> the <seealso cref="BpmnModelInstance"/> to write </param>
	  /// <exception cref="ModelException"> if the model cannot be written </exception>
	  /// <exception cref="ModelValidationException"> if the model is not valid </exception>
	  public static void writeModelToStream(Stream stream, BpmnModelInstance modelInstance)
	  {
		INSTANCE.doWriteModelToOutputStream(stream, modelInstance);
	  }

	  /// <summary>
	  /// Allows the conversion of a <seealso cref="BpmnModelInstance"/> to an <seealso cref="String"/>. It will
	  /// be validated before conversion.
	  /// </summary>
	  /// <param name="modelInstance">  the model instance to convert </param>
	  /// <returns> the XML string representation of the model instance </returns>
	  public static string convertToString(BpmnModelInstance modelInstance)
	  {
		return INSTANCE.doConvertToString(modelInstance);
	  }

	  /// <summary>
	  /// Validate model DOM document
	  /// </summary>
	  /// <param name="modelInstance"> the <seealso cref="BpmnModelInstance"/> to validate </param>
	  /// <exception cref="ModelValidationException"> if the model is not valid </exception>
	  public static void validateModel(BpmnModelInstance modelInstance)
	  {
		INSTANCE.doValidateModel(modelInstance);
	  }

	  /// <summary>
	  /// Allows creating an new, empty <seealso cref="BpmnModelInstance"/>.
	  /// </summary>
	  /// <returns> the empty model. </returns>
	  public static BpmnModelInstance createEmptyModel()
	  {
		return INSTANCE.doCreateEmptyModel();
	  }

	  public static ProcessBuilder createProcess()
	  {
		BpmnModelInstance modelInstance = INSTANCE.doCreateEmptyModel();
		Definitions definitions = modelInstance.newInstance(typeof(Definitions));
		definitions.TargetNamespace = BPMN20_NS;
		definitions.DomElement.registerNamespace("camunda", CAMUNDA_NS);
		modelInstance.Definitions = definitions;
		Process process = modelInstance.newInstance(typeof(Process));
		definitions.addChildElement(process);

		BpmnDiagram bpmnDiagram = modelInstance.newInstance(typeof(BpmnDiagram));

		BpmnPlane bpmnPlane = modelInstance.newInstance(typeof(BpmnPlane));
		bpmnPlane.BpmnElement = process;

		bpmnDiagram.addChildElement(bpmnPlane);
		definitions.addChildElement(bpmnDiagram);

		return process.builder();
	  }

	  public static ProcessBuilder createProcess(string processId)
	  {
		return createProcess().id(processId);
	  }

	  public static ProcessBuilder createExecutableProcess()
	  {
		return createProcess().executable();
	  }

	  public static ProcessBuilder createExecutableProcess(string processId)
	  {
		return createProcess(processId).executable();
	  }


	  /// <summary>
	  /// Register known types of the BPMN model
	  /// </summary>
	  protected internal Bpmn()
	  {
		bpmnModelBuilder = ModelBuilder.createInstance("BPMN Model");
		bpmnModelBuilder.alternativeNamespace(ACTIVITI_NS, CAMUNDA_NS);
		doRegisterTypes(bpmnModelBuilder);
		bpmnModel = bpmnModelBuilder.build();
	  }

	  protected internal virtual BpmnModelInstance doReadModelFromFile(File file)
	  {
		Stream @is = null;
		try
		{
		  @is = new FileStream(file, FileMode.Open, FileAccess.Read);
		  return doReadModelFromInputStream(@is);

		}
		catch (FileNotFoundException)
		{
		  throw new BpmnModelException("Cannot read model from file " + file + ": file does not exist.");

		}
		finally
		{
		  IoUtil.closeSilently(@is);

		}
	  }

	  protected internal virtual BpmnModelInstance doReadModelFromInputStream(Stream @is)
	  {
		return bpmnParser.parseModelFromStream(@is);
	  }

	  protected internal virtual void doWriteModelToFile(File file, BpmnModelInstance modelInstance)
	  {
		Stream os = null;
		try
		{
		  os = new FileStream(file, FileMode.Create, FileAccess.Write);
		  doWriteModelToOutputStream(os, modelInstance);
		}
		catch (FileNotFoundException)
		{
		  throw new BpmnModelException("Cannot write model to file " + file + ": file does not exist.");
		}
		finally
		{
		  IoUtil.closeSilently(os);
		}
	  }

	  protected internal virtual void doWriteModelToOutputStream(Stream os, BpmnModelInstance modelInstance)
	  {
		// validate DOM document
		doValidateModel(modelInstance);
		// write XML
		IoUtil.writeDocumentToOutputStream(modelInstance.Document, os);
	  }

	  protected internal virtual string doConvertToString(BpmnModelInstance modelInstance)
	  {
		// validate DOM document
		doValidateModel(modelInstance);
		// convert to XML string
		return IoUtil.convertXmlDocumentToString(modelInstance.Document);
	  }

	  protected internal virtual void doValidateModel(BpmnModelInstance modelInstance)
	  {
		bpmnParser.validateModel(modelInstance.Document);
	  }

	  protected internal virtual BpmnModelInstance doCreateEmptyModel()
	  {
		return bpmnParser.EmptyModel;
	  }

	  protected internal virtual void doRegisterTypes(ModelBuilder bpmnModelBuilder)
	  {
		ActivationConditionImpl.registerType(bpmnModelBuilder);
		ActivityImpl.registerType(bpmnModelBuilder);
		ArtifactImpl.registerType(bpmnModelBuilder);
		AssignmentImpl.registerType(bpmnModelBuilder);
		AssociationImpl.registerType(bpmnModelBuilder);
		AuditingImpl.registerType(bpmnModelBuilder);
		BaseElementImpl.registerType(bpmnModelBuilder);
		BoundaryEventImpl.registerType(bpmnModelBuilder);
		BusinessRuleTaskImpl.registerType(bpmnModelBuilder);
		CallableElementImpl.registerType(bpmnModelBuilder);
		CallActivityImpl.registerType(bpmnModelBuilder);
		CallConversationImpl.registerType(bpmnModelBuilder);
		CancelEventDefinitionImpl.registerType(bpmnModelBuilder);
		CatchEventImpl.registerType(bpmnModelBuilder);
		CategoryValueImpl.registerType(bpmnModelBuilder);
		CategoryValueRef.registerType(bpmnModelBuilder);
		ChildLaneSet.registerType(bpmnModelBuilder);
		CollaborationImpl.registerType(bpmnModelBuilder);
		CompensateEventDefinitionImpl.registerType(bpmnModelBuilder);
		ConditionImpl.registerType(bpmnModelBuilder);
		ConditionalEventDefinitionImpl.registerType(bpmnModelBuilder);
		CompletionConditionImpl.registerType(bpmnModelBuilder);
		ComplexBehaviorDefinitionImpl.registerType(bpmnModelBuilder);
		ComplexGatewayImpl.registerType(bpmnModelBuilder);
		ConditionExpressionImpl.registerType(bpmnModelBuilder);
		ConversationAssociationImpl.registerType(bpmnModelBuilder);
		ConversationImpl.registerType(bpmnModelBuilder);
		ConversationLinkImpl.registerType(bpmnModelBuilder);
		ConversationNodeImpl.registerType(bpmnModelBuilder);
		CorrelationKeyImpl.registerType(bpmnModelBuilder);
		CorrelationPropertyBindingImpl.registerType(bpmnModelBuilder);
		CorrelationPropertyImpl.registerType(bpmnModelBuilder);
		CorrelationPropertyRef.registerType(bpmnModelBuilder);
		CorrelationPropertyRetrievalExpressionImpl.registerType(bpmnModelBuilder);
		CorrelationSubscriptionImpl.registerType(bpmnModelBuilder);
		DataAssociationImpl.registerType(bpmnModelBuilder);
		DataInputAssociationImpl.registerType(bpmnModelBuilder);
		DataInputImpl.registerType(bpmnModelBuilder);
		DataInputRefs.registerType(bpmnModelBuilder);
		DataOutputAssociationImpl.registerType(bpmnModelBuilder);
		DataOutputImpl.registerType(bpmnModelBuilder);
		DataOutputRefs.registerType(bpmnModelBuilder);
		DataPath.registerType(bpmnModelBuilder);
		DataStateImpl.registerType(bpmnModelBuilder);
		DataObjectImpl.registerType(bpmnModelBuilder);
		DataObjectReferenceImpl.registerType(bpmnModelBuilder);
		DataStoreImpl.registerType(bpmnModelBuilder);
		DataStoreReferenceImpl.registerType(bpmnModelBuilder);
		DefinitionsImpl.registerType(bpmnModelBuilder);
		DocumentationImpl.registerType(bpmnModelBuilder);
		EndEventImpl.registerType(bpmnModelBuilder);
		EndPointImpl.registerType(bpmnModelBuilder);
		EndPointRef.registerType(bpmnModelBuilder);
		ErrorEventDefinitionImpl.registerType(bpmnModelBuilder);
		ErrorImpl.registerType(bpmnModelBuilder);
		ErrorRef.registerType(bpmnModelBuilder);
		EscalationImpl.registerType(bpmnModelBuilder);
		EscalationEventDefinitionImpl.registerType(bpmnModelBuilder);
		EventBasedGatewayImpl.registerType(bpmnModelBuilder);
		EventDefinitionImpl.registerType(bpmnModelBuilder);
		EventDefinitionRef.registerType(bpmnModelBuilder);
		EventImpl.registerType(bpmnModelBuilder);
		ExclusiveGatewayImpl.registerType(bpmnModelBuilder);
		ExpressionImpl.registerType(bpmnModelBuilder);
		ExtensionElementsImpl.registerType(bpmnModelBuilder);
		ExtensionImpl.registerType(bpmnModelBuilder);
		FlowElementImpl.registerType(bpmnModelBuilder);
		FlowNodeImpl.registerType(bpmnModelBuilder);
		FlowNodeRef.registerType(bpmnModelBuilder);
		FormalExpressionImpl.registerType(bpmnModelBuilder);
		From.registerType(bpmnModelBuilder);
		GatewayImpl.registerType(bpmnModelBuilder);
		GlobalConversationImpl.registerType(bpmnModelBuilder);
		HumanPerformerImpl.registerType(bpmnModelBuilder);
		ImportImpl.registerType(bpmnModelBuilder);
		InclusiveGatewayImpl.registerType(bpmnModelBuilder);
		Incoming.registerType(bpmnModelBuilder);
		InMessageRef.registerType(bpmnModelBuilder);
		InnerParticipantRef.registerType(bpmnModelBuilder);
		InputDataItemImpl.registerType(bpmnModelBuilder);
		InputSetImpl.registerType(bpmnModelBuilder);
		InputSetRefs.registerType(bpmnModelBuilder);
		InteractionNodeImpl.registerType(bpmnModelBuilder);
		InterfaceImpl.registerType(bpmnModelBuilder);
		InterfaceRef.registerType(bpmnModelBuilder);
		IntermediateCatchEventImpl.registerType(bpmnModelBuilder);
		IntermediateThrowEventImpl.registerType(bpmnModelBuilder);
		IoBindingImpl.registerType(bpmnModelBuilder);
		IoSpecificationImpl.registerType(bpmnModelBuilder);
		ItemAwareElementImpl.registerType(bpmnModelBuilder);
		ItemDefinitionImpl.registerType(bpmnModelBuilder);
		LaneImpl.registerType(bpmnModelBuilder);
		LaneSetImpl.registerType(bpmnModelBuilder);
		LinkEventDefinitionImpl.registerType(bpmnModelBuilder);
		LoopCardinalityImpl.registerType(bpmnModelBuilder);
		LoopCharacteristicsImpl.registerType(bpmnModelBuilder);
		LoopDataInputRef.registerType(bpmnModelBuilder);
		LoopDataOutputRef.registerType(bpmnModelBuilder);
		ManualTaskImpl.registerType(bpmnModelBuilder);
		MessageEventDefinitionImpl.registerType(bpmnModelBuilder);
		MessageFlowAssociationImpl.registerType(bpmnModelBuilder);
		MessageFlowImpl.registerType(bpmnModelBuilder);
		MessageFlowRef.registerType(bpmnModelBuilder);
		MessageImpl.registerType(bpmnModelBuilder);
		MessagePath.registerType(bpmnModelBuilder);
		ModelElementInstanceImpl.registerType(bpmnModelBuilder);
		MonitoringImpl.registerType(bpmnModelBuilder);
		MultiInstanceLoopCharacteristicsImpl.registerType(bpmnModelBuilder);
		OperationImpl.registerType(bpmnModelBuilder);
		OperationRef.registerType(bpmnModelBuilder);
		OptionalInputRefs.registerType(bpmnModelBuilder);
		OptionalOutputRefs.registerType(bpmnModelBuilder);
		OuterParticipantRef.registerType(bpmnModelBuilder);
		OutMessageRef.registerType(bpmnModelBuilder);
		Outgoing.registerType(bpmnModelBuilder);
		OutputDataItemImpl.registerType(bpmnModelBuilder);
		OutputSetImpl.registerType(bpmnModelBuilder);
		OutputSetRefs.registerType(bpmnModelBuilder);
		ParallelGatewayImpl.registerType(bpmnModelBuilder);
		ParticipantAssociationImpl.registerType(bpmnModelBuilder);
		ParticipantImpl.registerType(bpmnModelBuilder);
		ParticipantMultiplicityImpl.registerType(bpmnModelBuilder);
		ParticipantRef.registerType(bpmnModelBuilder);
		PartitionElement.registerType(bpmnModelBuilder);
		PerformerImpl.registerType(bpmnModelBuilder);
		PotentialOwnerImpl.registerType(bpmnModelBuilder);
		ProcessImpl.registerType(bpmnModelBuilder);
		PropertyImpl.registerType(bpmnModelBuilder);
		ReceiveTaskImpl.registerType(bpmnModelBuilder);
		RelationshipImpl.registerType(bpmnModelBuilder);
		RenderingImpl.registerType(bpmnModelBuilder);
		ResourceAssignmentExpressionImpl.registerType(bpmnModelBuilder);
		ResourceImpl.registerType(bpmnModelBuilder);
		ResourceParameterBindingImpl.registerType(bpmnModelBuilder);
		ResourceParameterImpl.registerType(bpmnModelBuilder);
		ResourceRef.registerType(bpmnModelBuilder);
		ResourceRoleImpl.registerType(bpmnModelBuilder);
		RootElementImpl.registerType(bpmnModelBuilder);
		ScriptImpl.registerType(bpmnModelBuilder);
		ScriptTaskImpl.registerType(bpmnModelBuilder);
		SendTaskImpl.registerType(bpmnModelBuilder);
		SequenceFlowImpl.registerType(bpmnModelBuilder);
		ServiceTaskImpl.registerType(bpmnModelBuilder);
		SignalEventDefinitionImpl.registerType(bpmnModelBuilder);
		SignalImpl.registerType(bpmnModelBuilder);
		Source.registerType(bpmnModelBuilder);
		SourceRef.registerType(bpmnModelBuilder);
		StartEventImpl.registerType(bpmnModelBuilder);
		SubConversationImpl.registerType(bpmnModelBuilder);
		SubProcessImpl.registerType(bpmnModelBuilder);
		SupportedInterfaceRef.registerType(bpmnModelBuilder);
		Supports.registerType(bpmnModelBuilder);
		Target.registerType(bpmnModelBuilder);
		TargetRef.registerType(bpmnModelBuilder);
		TaskImpl.registerType(bpmnModelBuilder);
		TerminateEventDefinitionImpl.registerType(bpmnModelBuilder);
		TextImpl.registerType(bpmnModelBuilder);
		TextAnnotationImpl.registerType(bpmnModelBuilder);
		ThrowEventImpl.registerType(bpmnModelBuilder);
		TimeCycleImpl.registerType(bpmnModelBuilder);
		TimeDateImpl.registerType(bpmnModelBuilder);
		TimeDurationImpl.registerType(bpmnModelBuilder);
		TimerEventDefinitionImpl.registerType(bpmnModelBuilder);
		To.registerType(bpmnModelBuilder);
		TransactionImpl.registerType(bpmnModelBuilder);
		Transformation.registerType(bpmnModelBuilder);
		UserTaskImpl.registerType(bpmnModelBuilder);
		WhileExecutingInputRefs.registerType(bpmnModelBuilder);
		WhileExecutingOutputRefs.registerType(bpmnModelBuilder);

		/// <summary>
		/// DC </summary>
		FontImpl.registerType(bpmnModelBuilder);
		PointImpl.registerType(bpmnModelBuilder);
		BoundsImpl.registerType(bpmnModelBuilder);

		/// <summary>
		/// DI </summary>
		DiagramImpl.registerType(bpmnModelBuilder);
		DiagramElementImpl.registerType(bpmnModelBuilder);
		EdgeImpl.registerType(bpmnModelBuilder);
		org.camunda.bpm.model.bpmn.impl.instance.di.ExtensionImpl.registerType(bpmnModelBuilder);
		LabelImpl.registerType(bpmnModelBuilder);
		LabeledEdgeImpl.registerType(bpmnModelBuilder);
		LabeledShapeImpl.registerType(bpmnModelBuilder);
		NodeImpl.registerType(bpmnModelBuilder);
		PlaneImpl.registerType(bpmnModelBuilder);
		ShapeImpl.registerType(bpmnModelBuilder);
		StyleImpl.registerType(bpmnModelBuilder);
		WaypointImpl.registerType(bpmnModelBuilder);

		/// <summary>
		/// BPMNDI </summary>
		BpmnDiagramImpl.registerType(bpmnModelBuilder);
		BpmnEdgeImpl.registerType(bpmnModelBuilder);
		BpmnLabelImpl.registerType(bpmnModelBuilder);
		BpmnLabelStyleImpl.registerType(bpmnModelBuilder);
		BpmnPlaneImpl.registerType(bpmnModelBuilder);
		BpmnShapeImpl.registerType(bpmnModelBuilder);

		/// <summary>
		/// camunda extensions </summary>
		CamundaConnectorImpl.registerType(bpmnModelBuilder);
		CamundaConnectorIdImpl.registerType(bpmnModelBuilder);
		CamundaConstraintImpl.registerType(bpmnModelBuilder);
		CamundaEntryImpl.registerType(bpmnModelBuilder);
		CamundaExecutionListenerImpl.registerType(bpmnModelBuilder);
		CamundaExpressionImpl.registerType(bpmnModelBuilder);
		CamundaFailedJobRetryTimeCycleImpl.registerType(bpmnModelBuilder);
		CamundaFieldImpl.registerType(bpmnModelBuilder);
		CamundaFormDataImpl.registerType(bpmnModelBuilder);
		CamundaFormFieldImpl.registerType(bpmnModelBuilder);
		CamundaFormPropertyImpl.registerType(bpmnModelBuilder);
		CamundaInImpl.registerType(bpmnModelBuilder);
		CamundaInputOutputImpl.registerType(bpmnModelBuilder);
		CamundaInputParameterImpl.registerType(bpmnModelBuilder);
		CamundaListImpl.registerType(bpmnModelBuilder);
		CamundaMapImpl.registerType(bpmnModelBuilder);
		CamundaOutputParameterImpl.registerType(bpmnModelBuilder);
		CamundaOutImpl.registerType(bpmnModelBuilder);
		CamundaPotentialStarterImpl.registerType(bpmnModelBuilder);
		CamundaPropertiesImpl.registerType(bpmnModelBuilder);
		CamundaPropertyImpl.registerType(bpmnModelBuilder);
		CamundaScriptImpl.registerType(bpmnModelBuilder);
		CamundaStringImpl.registerType(bpmnModelBuilder);
		CamundaTaskListenerImpl.registerType(bpmnModelBuilder);
		CamundaValidationImpl.registerType(bpmnModelBuilder);
		CamundaValueImpl.registerType(bpmnModelBuilder);
	  }

	  /// <returns> the <seealso cref="Model"/> instance to use </returns>
	  public virtual Model BpmnModel
	  {
		  get
		  {
			return bpmnModel;
		  }
		  set
		  {
			this.bpmnModel = value;
		  }
	  }

	  public virtual ModelBuilder BpmnModelBuilder
	  {
		  get
		  {
			return bpmnModelBuilder;
		  }
	  }


	}

}