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
namespace org.camunda.bpm.engine.impl.form.engine
{


	using FormData = org.camunda.bpm.engine.form.FormData;
	using FormField = org.camunda.bpm.engine.form.FormField;
	using FormFieldValidationConstraint = org.camunda.bpm.engine.form.FormFieldValidationConstraint;
	using FormProperty = org.camunda.bpm.engine.form.FormProperty;
	using StartFormData = org.camunda.bpm.engine.form.StartFormData;
	using TaskFormData = org.camunda.bpm.engine.form.TaskFormData;
	using BooleanFormType = org.camunda.bpm.engine.impl.form.type.BooleanFormType;
	using DateFormType = org.camunda.bpm.engine.impl.form.type.DateFormType;
	using EnumFormType = org.camunda.bpm.engine.impl.form.type.EnumFormType;
	using StringFormType = org.camunda.bpm.engine.impl.form.type.StringFormType;

	/// <summary>
	/// <para>A simple <seealso cref="FormEngine"/> implementaiton which renders
	/// forms as HTML such that they can be used as embedded forms
	/// inside camunda Tasklist.</para>
	/// 
	/// @author Daniel Meyer
	/// 
	/// </summary>
	public class HtmlFormEngine : FormEngine
	{

	  /* elements */
	  protected internal const string FORM_ELEMENT = "form";
	  protected internal const string DIV_ELEMENT = "div";
	  protected internal const string SPAN_ELEMENT = "span";
	  protected internal const string LABEL_ELEMENT = "label";
	  protected internal const string INPUT_ELEMENT = "input";
	  protected internal const string BUTTON_ELEMENT = "button";
	  protected internal const string SELECT_ELEMENT = "select";
	  protected internal const string OPTION_ELEMENT = "option";
	  protected internal const string I_ELEMENT = "i";
	  protected internal const string SCRIPT_ELEMENT = "script";

	  /* attributes */
	  protected internal const string NAME_ATTRIBUTE = "name";
	  protected internal const string CLASS_ATTRIBUTE = "class";
	  protected internal const string ROLE_ATTRIBUTE = "role";
	  protected internal const string FOR_ATTRIBUTE = "for";
	  protected internal const string VALUE_ATTRIBUTE = "value";
	  protected internal const string TYPE_ATTRIBUTE = "type";
	  protected internal const string SELECTED_ATTRIBUTE = "selected";

	  /* datepicker attributes*/
	  protected internal const string IS_OPEN_ATTRIBUTE = "is-open";
	  protected internal const string DATEPICKER_POPUP_ATTRIBUTE = "datepicker-popup";

	  /* camunda attributes */
	  protected internal const string CAM_VARIABLE_TYPE_ATTRIBUTE = "cam-variable-type";
	  protected internal const string CAM_VARIABLE_NAME_ATTRIBUTE = "cam-variable-name";
	  protected internal const string CAM_SCRIPT_ATTRIBUTE = "cam-script";
	  protected internal const string CAM_BUSINESS_KEY_ATTRIBUTE = "cam-business-key";

	  /* angular attributes*/
	  protected internal const string NG_CLICK_ATTRIBUTE = "ng-click";
	  protected internal const string NG_IF_ATTRIBUTE = "ng-if";
	  protected internal const string NG_SHOW_ATTRIBUTE = "ng-show";

	  /* classes */
	  protected internal const string FORM_GROUP_CLASS = "form-group";
	  protected internal const string FORM_CONTROL_CLASS = "form-control";
	  protected internal const string INPUT_GROUP_CLASS = "input-group";
	  protected internal const string INPUT_GROUP_BTN_CLASS = "input-group-btn";
	  protected internal const string BUTTON_DEFAULT_CLASS = "btn btn-default";
	  protected internal const string HAS_ERROR_CLASS = "has-error";
	  protected internal const string HELP_BLOCK_CLASS = "help-block";

	  /* input[type] */
	  protected internal const string TEXT_INPUT_TYPE = "text";
	  protected internal const string CHECKBOX_INPUT_TYPE = "checkbox";

	  /* button[type] */
	  protected internal const string BUTTON_BUTTON_TYPE = "button";

	  /* script[type] */
	  protected internal const string TEXT_FORM_SCRIPT_TYPE = "text/form-script";

	  /* glyphicons */
	  protected internal const string CALENDAR_GLYPHICON = "glyphicon glyphicon-calendar";

	  /* generated form name */
	  protected internal const string GENERATED_FORM_NAME = "generatedForm";
	  protected internal const string FORM_ROLE = "form";

	  /* error types */
	  protected internal const string REQUIRED_ERROR_TYPE = "required";
	  protected internal const string DATE_ERROR_TYPE = "date";

	  /* form element selector */
	  protected internal static readonly string FORM_ELEMENT_SELECTOR = "this." + GENERATED_FORM_NAME + ".%s";

	  /* expressions */
	  protected internal static readonly string INVALID_EXPRESSION = FORM_ELEMENT_SELECTOR + ".$invalid";
	  protected internal static readonly string DIRTY_EXPRESSION = FORM_ELEMENT_SELECTOR + ".$dirty";
	  protected internal static readonly string ERROR_EXPRESSION = FORM_ELEMENT_SELECTOR + ".$error";
	  protected internal static readonly string DATE_ERROR_EXPRESSION = ERROR_EXPRESSION + ".date";
	  protected internal static readonly string REQUIRED_ERROR_EXPRESSION = ERROR_EXPRESSION + ".required";
	  protected internal static readonly string TYPE_ERROR_EXPRESSION = ERROR_EXPRESSION + ".camVariableType";

	  /* JavaScript snippets */
	  protected internal const string DATE_FIELD_OPENED_ATTRIBUTE = "dateFieldOpened%s";
	  protected internal const string OPEN_DATEPICKER_SNIPPET = "$scope.open%s = function ($event) { $event.preventDefault(); $event.stopPropagation(); $scope.dateFieldOpened%s = true; };";
	  protected internal const string OPEN_DATEPICKER_FUNCTION_SNIPPET = "open%s($event)";

	  /* messages */
	  protected internal const string REQUIRED_FIELD_MESSAGE = "Required field";
	  protected internal const string TYPE_FIELD_MESSAGE = "Only a %s value is allowed";
	  protected internal const string INVALID_DATE_FIELD_MESSAGE = "Invalid date format: the date should have the pattern ";

	  protected internal const string DATE_PATTERN_ATTRIBUTE = "datePattern";

	  public virtual string Name
	  {
		  get
		  {
			return "html";
		  }
	  }

	  public virtual object renderStartForm(StartFormData startForm)
	  {
		return renderFormData(startForm);
	  }

	  public virtual object renderTaskForm(TaskFormData taskForm)
	  {
		return renderFormData(taskForm);
	  }

	  protected internal virtual string renderFormData(FormData formData)
	  {

		if (formData == null || (formData.FormFields == null || formData.FormFields.Count == 0) && (formData.FormProperties == null || formData.FormProperties.Count == 0))
		{
		  return null;

		}
		else
		{
		  HtmlElementWriter formElement = (new HtmlElementWriter(FORM_ELEMENT)).attribute(NAME_ATTRIBUTE, GENERATED_FORM_NAME).attribute(ROLE_ATTRIBUTE, FORM_ROLE);

		  HtmlDocumentBuilder documentBuilder = new HtmlDocumentBuilder(formElement);

		  // render fields
		  foreach (FormField formField in formData.FormFields)
		  {
			renderFormField(formField, documentBuilder);
		  }

		  // render deprecated form properties
		  foreach (FormProperty formProperty in formData.FormProperties)
		  {
			renderFormField(new FormPropertyAdapter(formProperty), documentBuilder);
		  }

		  // end document element
		  documentBuilder.endElement();

		  return documentBuilder.HtmlString;

		}
	  }

	  protected internal virtual void renderFormField(FormField formField, HtmlDocumentBuilder documentBuilder)
	  {
		// start group
		HtmlElementWriter divElement = (new HtmlElementWriter(DIV_ELEMENT)).attribute(CLASS_ATTRIBUTE, FORM_GROUP_CLASS);

		documentBuilder.startElement(divElement);

		string formFieldId = formField.Id;
		string formFieldLabel = formField.Label;

		// write label
		if (!string.ReferenceEquals(formFieldLabel, null) && formFieldLabel.Length > 0)
		{

		  HtmlElementWriter labelElement = (new HtmlElementWriter(LABEL_ELEMENT)).attribute(FOR_ATTRIBUTE, formFieldId).textContent(formFieldLabel);

		  // <label for="...">...</label>
		  documentBuilder.startElement(labelElement).endElement();
		}

		// render form control
		if (isEnum(formField))
		{
		  // <select ...>
		  renderSelectBox(formField, documentBuilder);

		}
		else if (isDate(formField))
		{

		  renderDatePicker(formField, documentBuilder);

		}
		else
		{
		  // <input ...>
		  renderInputField(formField, documentBuilder);

		}

		renderInvalidMessageElement(formField, documentBuilder);

		// end group
		documentBuilder.endElement();
	  }

	  protected internal virtual HtmlElementWriter createInputField(FormField formField)
	  {
		HtmlElementWriter inputField = new HtmlElementWriter(INPUT_ELEMENT, true);

		addCommonFormFieldAttributes(formField, inputField);

		inputField.attribute(TYPE_ATTRIBUTE, TEXT_INPUT_TYPE);

		return inputField;
	  }

	  protected internal virtual void renderDatePicker(FormField formField, HtmlDocumentBuilder documentBuilder)
	  {
		bool isReadOnly = isReadOnly(formField);

		// start input-group
		HtmlElementWriter inputGroupDivElement = (new HtmlElementWriter(DIV_ELEMENT)).attribute(CLASS_ATTRIBUTE, INPUT_GROUP_CLASS);

		string formFieldId = formField.Id;

		// <div>
		documentBuilder.startElement(inputGroupDivElement);

		// input field
		HtmlElementWriter inputField = createInputField(formField);

		string dateFormat = (string) formField.Type.getInformation(DATE_PATTERN_ATTRIBUTE);
		if (!isReadOnly)
		{
		  inputField.attribute(DATEPICKER_POPUP_ATTRIBUTE, dateFormat).attribute(IS_OPEN_ATTRIBUTE, string.format(DATE_FIELD_OPENED_ATTRIBUTE, formFieldId));
		}

		// <input ... />
		documentBuilder.startElement(inputField).endElement();


		// if form field is read only, do not render date picker open button
		if (!isReadOnly)
		{

		  // input addon
		  HtmlElementWriter addonElement = (new HtmlElementWriter(DIV_ELEMENT)).attribute(CLASS_ATTRIBUTE, INPUT_GROUP_BTN_CLASS);

		  // <div>
		  documentBuilder.startElement(addonElement);

		  // button to open date picker
		  HtmlElementWriter buttonElement = (new HtmlElementWriter(BUTTON_ELEMENT)).attribute(TYPE_ATTRIBUTE, BUTTON_BUTTON_TYPE).attribute(CLASS_ATTRIBUTE, BUTTON_DEFAULT_CLASS).attribute(NG_CLICK_ATTRIBUTE, string.format(OPEN_DATEPICKER_FUNCTION_SNIPPET, formFieldId));

		  // <button>
		  documentBuilder.startElement(buttonElement);

		  HtmlElementWriter iconElement = (new HtmlElementWriter(I_ELEMENT)).attribute(CLASS_ATTRIBUTE, CALENDAR_GLYPHICON);

		  // <i ...></i>
		  documentBuilder.startElement(iconElement).endElement();

		  // </button>
		  documentBuilder.endElement();

		  // </div>
		  documentBuilder.endElement();


		  HtmlElementWriter scriptElement = (new HtmlElementWriter(SCRIPT_ELEMENT)).attribute(CAM_SCRIPT_ATTRIBUTE, null).attribute(TYPE_ATTRIBUTE, TEXT_FORM_SCRIPT_TYPE).textContent(string.format(OPEN_DATEPICKER_SNIPPET, formFieldId, formFieldId));

		  // <script ...> </script>
		  documentBuilder.startElement(scriptElement).endElement();

		}

		// </div>
		documentBuilder.endElement();

	  }

	  protected internal virtual void renderInputField(FormField formField, HtmlDocumentBuilder documentBuilder)
	  {
		HtmlElementWriter inputField = new HtmlElementWriter(INPUT_ELEMENT, true);
		addCommonFormFieldAttributes(formField, inputField);

		string inputType = !isBoolean(formField) ? TEXT_INPUT_TYPE : CHECKBOX_INPUT_TYPE;

		inputField.attribute(TYPE_ATTRIBUTE, inputType);

		// add default value
		object defaultValue = formField.DefaultValue;
		if (defaultValue != null)
		{
		  inputField.attribute(VALUE_ATTRIBUTE, defaultValue.ToString());
		}

		// <input ... />
		documentBuilder.startElement(inputField).endElement();
	  }

	  protected internal virtual void renderSelectBox(FormField formField, HtmlDocumentBuilder documentBuilder)
	  {
		HtmlElementWriter selectBox = new HtmlElementWriter(SELECT_ELEMENT, false);

		addCommonFormFieldAttributes(formField, selectBox);

		// <select ...>
		documentBuilder.startElement(selectBox);

		// <option ...>
		renderSelectOptions(formField, documentBuilder);

		// </select>
		documentBuilder.endElement();
	  }

	  protected internal virtual void renderSelectOptions(FormField formField, HtmlDocumentBuilder documentBuilder)
	  {
		EnumFormType enumFormType = (EnumFormType) formField.Type;
		IDictionary<string, string> values = enumFormType.Values;

		foreach (KeyValuePair<string, string> value in values.SetOfKeyValuePairs())
		{
		  // <option>
		  HtmlElementWriter option = (new HtmlElementWriter(OPTION_ELEMENT, false)).attribute(VALUE_ATTRIBUTE, value.Key).textContent(value.Value);

		  documentBuilder.startElement(option).endElement();
		}
	  }

	  protected internal virtual void renderInvalidMessageElement(FormField formField, HtmlDocumentBuilder documentBuilder)
	  {
		HtmlElementWriter divElement = new HtmlElementWriter(DIV_ELEMENT);

		string formFieldId = formField.Id;
		string ifExpression = string.format(INVALID_EXPRESSION + " && " + DIRTY_EXPRESSION, formFieldId, formFieldId);

		divElement.attribute(NG_IF_ATTRIBUTE, ifExpression).attribute(CLASS_ATTRIBUTE, HAS_ERROR_CLASS);

		// <div ng-if="....$invalid && ....$dirty"...>
		documentBuilder.startElement(divElement);

		if (!isDate(formField))
		{
		  renderInvalidValueMessage(formField, documentBuilder);
		  renderInvalidTypeMessage(formField, documentBuilder);

		}
		else
		{
		  renderInvalidDateMessage(formField, documentBuilder);
		}

		documentBuilder.endElement();
	  }

	  protected internal virtual void renderInvalidValueMessage(FormField formField, HtmlDocumentBuilder documentBuilder)
	  {
		HtmlElementWriter divElement = new HtmlElementWriter(DIV_ELEMENT);

		string formFieldId = formField.Id;

		string expression = string.format(REQUIRED_ERROR_EXPRESSION, formFieldId);

		divElement.attribute(NG_SHOW_ATTRIBUTE, expression).attribute(CLASS_ATTRIBUTE, HELP_BLOCK_CLASS).textContent(REQUIRED_FIELD_MESSAGE);

		documentBuilder.startElement(divElement).endElement();
	  }

	  protected internal virtual void renderInvalidTypeMessage(FormField formField, HtmlDocumentBuilder documentBuilder)
	  {
		HtmlElementWriter divElement = new HtmlElementWriter(DIV_ELEMENT);

		string formFieldId = formField.Id;

		string expression = string.format(TYPE_ERROR_EXPRESSION, formFieldId);

		string typeName = formField.TypeName;

		if (isEnum(formField))
		{
		  typeName = StringFormType.TYPE_NAME;
		}

		divElement.attribute(NG_SHOW_ATTRIBUTE, expression).attribute(CLASS_ATTRIBUTE, HELP_BLOCK_CLASS).textContent(string.format(TYPE_FIELD_MESSAGE, typeName));

		documentBuilder.startElement(divElement).endElement();
	  }

	  protected internal virtual void renderInvalidDateMessage(FormField formField, HtmlDocumentBuilder documentBuilder)
	  {
		string formFieldId = formField.Id;

		HtmlElementWriter firstDivElement = new HtmlElementWriter(DIV_ELEMENT);

		string firstExpression = string.format(REQUIRED_ERROR_EXPRESSION + " && !" + DATE_ERROR_EXPRESSION, formFieldId, formFieldId);

		firstDivElement.attribute(NG_SHOW_ATTRIBUTE, firstExpression).attribute(CLASS_ATTRIBUTE, HELP_BLOCK_CLASS).textContent(REQUIRED_FIELD_MESSAGE);

		documentBuilder.startElement(firstDivElement).endElement();

		HtmlElementWriter secondDivElement = new HtmlElementWriter(DIV_ELEMENT);

		string secondExpression = string.format(DATE_ERROR_EXPRESSION, formFieldId);

		secondDivElement.attribute(NG_SHOW_ATTRIBUTE, secondExpression).attribute(CLASS_ATTRIBUTE, HELP_BLOCK_CLASS).textContent(INVALID_DATE_FIELD_MESSAGE + "'" + formField.Type.getInformation(DATE_PATTERN_ATTRIBUTE) + "'");

		documentBuilder.startElement(secondDivElement).endElement();
	  }

	  protected internal virtual void addCommonFormFieldAttributes(FormField formField, HtmlElementWriter formControl)
	  {

		string typeName = formField.TypeName;

		if (isEnum(formField) || isDate(formField))
		{
		  typeName = StringFormType.TYPE_NAME;
		}

		typeName = typeName.Substring(0, 1).ToUpper() + typeName.Substring(1);

		string formFieldId = formField.Id;

		formControl.attribute(CLASS_ATTRIBUTE, FORM_CONTROL_CLASS).attribute(NAME_ATTRIBUTE, formFieldId);

		if (!formField.BusinessKey)
		{
		  formControl.attribute(CAM_VARIABLE_TYPE_ATTRIBUTE, typeName).attribute(CAM_VARIABLE_NAME_ATTRIBUTE, formFieldId);
		}
		else
		{
		  formControl.attribute(CAM_BUSINESS_KEY_ATTRIBUTE, null);
		}

		// add validation constraints
		foreach (FormFieldValidationConstraint constraint in formField.ValidationConstraints)
		{
		  string constraintName = constraint.Name;
		  string configuration = (string) constraint.Configuration;
		  formControl.attribute(constraintName, configuration);
		}
	  }

	  // helper /////////////////////////////////////////////////////////////////////////////////////

	  protected internal virtual bool isEnum(FormField formField)
	  {
		return EnumFormType.TYPE_NAME.Equals(formField.TypeName);
	  }

	  protected internal virtual bool isDate(FormField formField)
	  {
		return DateFormType.TYPE_NAME.Equals(formField.TypeName);
	  }

	  protected internal virtual bool isBoolean(FormField formField)
	  {
		return BooleanFormType.TYPE_NAME.Equals(formField.TypeName);
	  }

	  protected internal virtual bool isReadOnly(FormField formField)
	  {
		IList<FormFieldValidationConstraint> validationConstraints = formField.ValidationConstraints;
		if (validationConstraints != null)
		{
		  foreach (FormFieldValidationConstraint validationConstraint in validationConstraints)
		  {
			if ("readonly".Equals(validationConstraint.Name))
			{
			  return true;
			}
		  }
		}
		return false;
	  }

	}

}