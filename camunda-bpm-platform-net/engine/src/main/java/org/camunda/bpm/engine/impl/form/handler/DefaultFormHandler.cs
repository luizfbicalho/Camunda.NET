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
namespace org.camunda.bpm.engine.impl.form.handler
{

	using Expression = org.camunda.bpm.engine.@delegate.Expression;
	using VariableScope = org.camunda.bpm.engine.@delegate.VariableScope;
	using FormField = org.camunda.bpm.engine.form.FormField;
	using FormProperty = org.camunda.bpm.engine.form.FormProperty;
	using BpmnParse = org.camunda.bpm.engine.impl.bpmn.parser.BpmnParse;
	using ProcessEngineConfigurationImpl = org.camunda.bpm.engine.impl.cfg.ProcessEngineConfigurationImpl;
	using Context = org.camunda.bpm.engine.impl.context.Context;
	using ExpressionManager = org.camunda.bpm.engine.impl.el.ExpressionManager;
	using AbstractFormFieldType = org.camunda.bpm.engine.impl.form.type.AbstractFormFieldType;
	using FormTypes = org.camunda.bpm.engine.impl.form.type.FormTypes;
	using FormFieldValidator = org.camunda.bpm.engine.impl.form.validator.FormFieldValidator;
	using HistoryLevel = org.camunda.bpm.engine.impl.history.HistoryLevel;
	using HistoryEvent = org.camunda.bpm.engine.impl.history.@event.HistoryEvent;
	using HistoryEventProcessor = org.camunda.bpm.engine.impl.history.@event.HistoryEventProcessor;
	using HistoryEventTypes = org.camunda.bpm.engine.impl.history.@event.HistoryEventTypes;
	using HistoryEventProducer = org.camunda.bpm.engine.impl.history.producer.HistoryEventProducer;
	using DeploymentEntity = org.camunda.bpm.engine.impl.persistence.entity.DeploymentEntity;
	using ExecutionEntity = org.camunda.bpm.engine.impl.persistence.entity.ExecutionEntity;
	using ProcessDefinitionEntity = org.camunda.bpm.engine.impl.persistence.entity.ProcessDefinitionEntity;
	using TaskEntity = org.camunda.bpm.engine.impl.persistence.entity.TaskEntity;
	using Element = org.camunda.bpm.engine.impl.util.xml.Element;
	using VariableMap = org.camunda.bpm.engine.variable.VariableMap;
	using VariableMapImpl = org.camunda.bpm.engine.variable.impl.VariableMapImpl;
	using SerializableValue = org.camunda.bpm.engine.variable.value.SerializableValue;
	using TypedValue = org.camunda.bpm.engine.variable.value.TypedValue;


	/// <summary>
	/// @author Tom Baeyens
	/// @author Daniel Meyer
	/// </summary>
	public class DefaultFormHandler : FormHandler
	{

	  public const string FORM_FIELD_ELEMENT = "formField";
	  public const string FORM_PROPERTY_ELEMENT = "formProperty";
	  private const string BUSINESS_KEY_ATTRIBUTE = "businessKey";

	  protected internal string deploymentId;
	  protected internal string businessKeyFieldId;

	  protected internal IList<FormPropertyHandler> formPropertyHandlers = new List<FormPropertyHandler>();

	  protected internal IList<FormFieldHandler> formFieldHandlers = new List<FormFieldHandler>();

	  public virtual void parseConfiguration(Element activityElement, DeploymentEntity deployment, ProcessDefinitionEntity processDefinition, BpmnParse bpmnParse)
	  {
		this.deploymentId = deployment.Id;

		ExpressionManager expressionManager = Context.ProcessEngineConfiguration.ExpressionManager;

		Element extensionElement = activityElement.element("extensionElements");
		if (extensionElement != null)
		{

		  // provide support for deprecated form properties
		  parseFormProperties(bpmnParse, expressionManager, extensionElement);

		  // provide support for new form field metadata
		  parseFormData(bpmnParse, expressionManager, extensionElement);
		}
	  }

	  protected internal virtual void parseFormData(BpmnParse bpmnParse, ExpressionManager expressionManager, Element extensionElement)
	  {
		Element formData = extensionElement.elementNS(BpmnParse.CAMUNDA_BPMN_EXTENSIONS_NS, "formData");
		if (formData != null)
		{
		  this.businessKeyFieldId = formData.attribute(BUSINESS_KEY_ATTRIBUTE);
		  parseFormFields(formData, bpmnParse, expressionManager);
		}
	  }

	  protected internal virtual void parseFormFields(Element formData, BpmnParse bpmnParse, ExpressionManager expressionManager)
	  {
		// parse fields:
		IList<Element> formFields = formData.elementsNS(BpmnParse.CAMUNDA_BPMN_EXTENSIONS_NS, FORM_FIELD_ELEMENT);
		foreach (Element formField in formFields)
		{
		  parseFormField(formField, bpmnParse, expressionManager);
		}
	  }

	  protected internal virtual void parseFormField(Element formField, BpmnParse bpmnParse, ExpressionManager expressionManager)
	  {

		FormFieldHandler formFieldHandler = new FormFieldHandler();

		// parse Id
		string id = formField.attribute("id");
		if (string.ReferenceEquals(id, null) || id.Length == 0)
		{
		  bpmnParse.addError("attribute id must be set for FormFieldGroup and must have a non-empty value", formField);
		}
		else
		{
		  formFieldHandler.Id = id;
		}

		if (id.Equals(businessKeyFieldId))
		{
		  formFieldHandler.BusinessKey = true;
		}

		// parse name
		string name = formField.attribute("label");
		if (!string.ReferenceEquals(name, null))
		{
		  Expression nameExpression = expressionManager.createExpression(name);
		  formFieldHandler.Label = nameExpression;
		}

		// parse properties
		parseProperties(formField, formFieldHandler, bpmnParse, expressionManager);

		// parse validation
		parseValidation(formField, formFieldHandler, bpmnParse, expressionManager);

		// parse type
		FormTypes formTypes = FormTypes;
		AbstractFormFieldType formType = formTypes.parseFormPropertyType(formField, bpmnParse);
		formFieldHandler.setType(formType);

		// parse default value
		string defaultValue = formField.attribute("defaultValue");
		if (!string.ReferenceEquals(defaultValue, null))
		{
		  Expression defaultValueExpression = expressionManager.createExpression(defaultValue);
		  formFieldHandler.DefaultValueExpression = defaultValueExpression;
		}

		formFieldHandlers.Add(formFieldHandler);

	  }

	  protected internal virtual void parseProperties(Element formField, FormFieldHandler formFieldHandler, BpmnParse bpmnParse, ExpressionManager expressionManager)
	  {

		Element propertiesElement = formField.elementNS(BpmnParse.CAMUNDA_BPMN_EXTENSIONS_NS, "properties");

		if (propertiesElement != null)
		{
		  IList<Element> propertyElements = propertiesElement.elementsNS(BpmnParse.CAMUNDA_BPMN_EXTENSIONS_NS, "property");

		  // use linked hash map to preserve item ordering as provided in XML
		  IDictionary<string, string> propertyMap = new LinkedHashMap<string, string>();
		  foreach (Element property in propertyElements)
		  {
			string id = property.attribute("id");
			string value = property.attribute("value");
			propertyMap[id] = value;
		  }

		  formFieldHandler.Properties = propertyMap;
		}

	  }

	  protected internal virtual void parseValidation(Element formField, FormFieldHandler formFieldHandler, BpmnParse bpmnParse, ExpressionManager expressionManager)
	  {

		Element validationElement = formField.elementNS(BpmnParse.CAMUNDA_BPMN_EXTENSIONS_NS, "validation");

		if (validationElement != null)
		{
		  IList<Element> constraintElements = validationElement.elementsNS(BpmnParse.CAMUNDA_BPMN_EXTENSIONS_NS, "constraint");

		  foreach (Element property in constraintElements)
		  {
			 FormFieldValidator validator = Context.ProcessEngineConfiguration.FormValidators.createValidator(property, bpmnParse, expressionManager);

			 string validatorName = property.attribute("name");
			 string validatorConfig = property.attribute("config");

			 if (validator != null)
			 {
			   FormFieldValidationConstraintHandler handler = new FormFieldValidationConstraintHandler();
			   handler.Name = validatorName;
			   handler.Config = validatorConfig;
			   handler.Validator = validator;
			   formFieldHandler.ValidationHandlers.Add(handler);
			 }
		  }
		}
	  }


	  protected internal virtual FormTypes FormTypes
	  {
		  get
		  {
			FormTypes formTypes = Context.ProcessEngineConfiguration.FormTypes;
			return formTypes;
		  }
	  }

	  protected internal virtual void parseFormProperties(BpmnParse bpmnParse, ExpressionManager expressionManager, Element extensionElement)
	  {
		FormTypes formTypes = FormTypes;

		IList<Element> formPropertyElements = extensionElement.elementsNS(BpmnParse.CAMUNDA_BPMN_EXTENSIONS_NS, FORM_PROPERTY_ELEMENT);
		foreach (Element formPropertyElement in formPropertyElements)
		{
		  FormPropertyHandler formPropertyHandler = new FormPropertyHandler();

		  string id = formPropertyElement.attribute("id");
		  if (string.ReferenceEquals(id, null))
		  {
			bpmnParse.addError("attribute 'id' is required", formPropertyElement);
		  }
		  formPropertyHandler.Id = id;

		  string name = formPropertyElement.attribute("name");
		  formPropertyHandler.Name = name;

		  AbstractFormFieldType type = formTypes.parseFormPropertyType(formPropertyElement, bpmnParse);
		  formPropertyHandler.setType(type);

		  string requiredText = formPropertyElement.attribute("required", "false");
		  bool? required = bpmnParse.parseBooleanAttribute(requiredText);
		  if (required != null)
		  {
			formPropertyHandler.Required = required.Value;
		  }
		  else
		  {
			bpmnParse.addError("attribute 'required' must be one of {on|yes|true|enabled|active|off|no|false|disabled|inactive}", formPropertyElement);
		  }

		  string readableText = formPropertyElement.attribute("readable", "true");
		  bool? readable = bpmnParse.parseBooleanAttribute(readableText);
		  if (readable != null)
		  {
			formPropertyHandler.Readable = readable.Value;
		  }
		  else
		  {
			bpmnParse.addError("attribute 'readable' must be one of {on|yes|true|enabled|active|off|no|false|disabled|inactive}", formPropertyElement);
		  }

		  string writableText = formPropertyElement.attribute("writable", "true");
		  bool? writable = bpmnParse.parseBooleanAttribute(writableText);
		  if (writable != null)
		  {
			formPropertyHandler.Writable = writable.Value;
		  }
		  else
		  {
			bpmnParse.addError("attribute 'writable' must be one of {on|yes|true|enabled|active|off|no|false|disabled|inactive}", formPropertyElement);
		  }

		  string variableName = formPropertyElement.attribute("variable");
		  formPropertyHandler.VariableName = variableName;

		  string expressionText = formPropertyElement.attribute("expression");
		  if (!string.ReferenceEquals(expressionText, null))
		  {
			Expression expression = expressionManager.createExpression(expressionText);
			formPropertyHandler.VariableExpression = expression;
		  }

		  string defaultExpressionText = formPropertyElement.attribute("default");
		  if (!string.ReferenceEquals(defaultExpressionText, null))
		  {
			Expression defaultExpression = expressionManager.createExpression(defaultExpressionText);
			formPropertyHandler.DefaultExpression = defaultExpression;
		  }

		  formPropertyHandlers.Add(formPropertyHandler);
		}
	  }

	  protected internal virtual void initializeFormProperties(FormDataImpl formData, ExecutionEntity execution)
	  {
		IList<FormProperty> formProperties = new List<FormProperty>();
		foreach (FormPropertyHandler formPropertyHandler in formPropertyHandlers)
		{
		  if (formPropertyHandler.Readable)
		  {
			FormProperty formProperty = formPropertyHandler.createFormProperty(execution);
			formProperties.Add(formProperty);
		  }
		}
		formData.FormProperties = formProperties;
	  }

	  protected internal virtual void initializeFormFields(FormDataImpl taskFormData, ExecutionEntity execution)
	  {
		// add form fields
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.List<org.camunda.bpm.engine.form.FormField> formFields = taskFormData.getFormFields();
		IList<FormField> formFields = taskFormData.FormFields;
		foreach (FormFieldHandler formFieldHandler in formFieldHandlers)
		{
		  formFields.Add(formFieldHandler.createFormField(execution));
		}
	  }

	  public virtual void submitFormVariables(VariableMap properties, VariableScope variableScope)
	  {
		bool userOperationLogEnabled = Context.CommandContext.UserOperationLogEnabled;
		Context.CommandContext.enableUserOperationLog();

		VariableMap propertiesCopy = new VariableMapImpl(properties);

		// support legacy form properties
		foreach (FormPropertyHandler formPropertyHandler in formPropertyHandlers)
		{
		  // submitFormProperty will remove all the keys which it takes care of
		  formPropertyHandler.submitFormProperty(variableScope, propertiesCopy);
		}

		// support form data:
		foreach (FormFieldHandler formFieldHandler in formFieldHandlers)
		{
		  if (!formFieldHandler.BusinessKey)
		  {
			formFieldHandler.handleSubmit(variableScope, propertiesCopy, properties);
		  }
		}

		// any variables passed in which are not handled by form-fields or form
		// properties are added to the process as variables
		foreach (string propertyId in propertiesCopy.Keys)
		{
		  variableScope.setVariable(propertyId, propertiesCopy.getValueTyped(propertyId));
		}

		fireFormPropertyHistoryEvents(properties, variableScope);

		Context.CommandContext.LogUserOperationEnabled = userOperationLogEnabled;
	  }

	  protected internal virtual void fireFormPropertyHistoryEvents(VariableMap properties, VariableScope variableScope)
	  {
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.camunda.bpm.engine.impl.cfg.ProcessEngineConfigurationImpl processEngineConfiguration = org.camunda.bpm.engine.impl.context.Context.getProcessEngineConfiguration();
		ProcessEngineConfigurationImpl processEngineConfiguration = Context.ProcessEngineConfiguration;
		HistoryLevel historyLevel = processEngineConfiguration.HistoryLevel;

		if (historyLevel.isHistoryEventProduced(HistoryEventTypes.FORM_PROPERTY_UPDATE, variableScope))
		{

		  // fire history events
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.camunda.bpm.engine.impl.persistence.entity.ExecutionEntity executionEntity;
		  ExecutionEntity executionEntity;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final String taskId;
		  string taskId;
		  if (variableScope is ExecutionEntity)
		  {
			executionEntity = (ExecutionEntity) variableScope;
			taskId = null;
		  }
		  else if (variableScope is TaskEntity)
		  {
			TaskEntity task = (TaskEntity) variableScope;
			executionEntity = task.getExecution();
			taskId = task.Id;
		  }
		  else
		  {
			executionEntity = null;
			taskId = null;
		  }

		  if (executionEntity != null)
		  {
			foreach (String variableName in properties.Keys)
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.camunda.bpm.engine.variable.value.TypedValue value = properties.getValueTyped(variableName);
			  TypedValue value = properties.getValueTyped(variableName);

			  // NOTE: SerializableValues are never stored as form properties
			  if (!(value is SerializableValue) && value.Value != null && value.Value is String)
			  {
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final String stringValue = (String) value.getValue();
				string stringValue = (String) value.Value;

				HistoryEventProcessor.processHistoryEvents(new HistoryEventCreatorAnonymousInnerClass(this, executionEntity, taskId, stringValue));
			  }
			}
		  }
		}
	  }

	  private class HistoryEventCreatorAnonymousInnerClass : HistoryEventProcessor.HistoryEventCreator
	  {
		  private readonly DefaultFormHandler outerInstance;

		  private ExecutionEntity executionEntity;
		  private string taskId;
		  private string stringValue;

		  public HistoryEventCreatorAnonymousInnerClass(DefaultFormHandler outerInstance, ExecutionEntity executionEntity, string taskId, string stringValue)
		  {
			  this.outerInstance = outerInstance;
			  this.executionEntity = executionEntity;
			  this.taskId = taskId;
			  this.stringValue = stringValue;
		  }

		  public override HistoryEvent createHistoryEvent(HistoryEventProducer producer)
		  {
			return producer.createFormPropertyUpdateEvt(executionEntity, variableName, stringValue, taskId);
		  }
	  }


	  // getters and setters //////////////////////////////////////////////////////

	  public virtual string DeploymentId
	  {
		  get
		  {
			return deploymentId;
		  }
		  set
		  {
			this.deploymentId = value;
		  }
	  }


	  public virtual IList<FormPropertyHandler> FormPropertyHandlers
	  {
		  get
		  {
			return formPropertyHandlers;
		  }
		  set
		  {
			this.formPropertyHandlers = value;
		  }
	  }


	  public virtual string BusinessKeyFieldId
	  {
		  get
		  {
			return businessKeyFieldId;
		  }
		  set
		  {
			this.businessKeyFieldId = value;
		  }
	  }

	}

}