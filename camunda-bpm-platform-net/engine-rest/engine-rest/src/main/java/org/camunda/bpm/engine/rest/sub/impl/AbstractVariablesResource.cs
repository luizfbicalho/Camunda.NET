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
namespace org.camunda.bpm.engine.rest.sub.impl
{
	using JavaType = com.fasterxml.jackson.databind.JavaType;
	using ObjectMapper = com.fasterxml.jackson.databind.ObjectMapper;
	using TypeFactory = com.fasterxml.jackson.databind.type.TypeFactory;
	using PatchVariablesDto = org.camunda.bpm.engine.rest.dto.PatchVariablesDto;
	using VariableValueDto = org.camunda.bpm.engine.rest.dto.VariableValueDto;
	using InvalidRequestException = org.camunda.bpm.engine.rest.exception.InvalidRequestException;
	using RestException = org.camunda.bpm.engine.rest.exception.RestException;
	using MultipartFormData = org.camunda.bpm.engine.rest.mapper.MultipartFormData;
	using FormPart = org.camunda.bpm.engine.rest.mapper.MultipartFormData.FormPart;
	using VariableMap = org.camunda.bpm.engine.variable.VariableMap;
	using Variables = org.camunda.bpm.engine.variable.Variables;
	using TypedValue = org.camunda.bpm.engine.variable.value.TypedValue;



	public abstract class AbstractVariablesResource : VariableResource
	{
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: public abstract org.camunda.bpm.engine.rest.dto.VariableValueDto getVariable(String variableName, @DefaultValue("true") boolean deserializeValue);
		public abstract VariableValueDto getVariable(string variableName, bool deserializeValue);

	  protected internal const string DEFAULT_BINARY_VALUE_TYPE = "Bytes";

	  protected internal ProcessEngine engine;
	  protected internal string resourceId;
	  protected internal ObjectMapper objectMapper;

	  public AbstractVariablesResource(ProcessEngine engine, string resourceId, ObjectMapper objectMapper)
	  {
		this.engine = engine;
		this.resourceId = resourceId;
		this.objectMapper = objectMapper;
	  }

	  public virtual IDictionary<string, VariableValueDto> getVariables(bool deserializeValues)
	  {

		VariableMap variables = getVariableEntities(deserializeValues);

		return VariableValueDto.fromMap(variables);
	  }

	  public virtual VariableValueDto getVariable(string variableName, bool deserializeValue)
	  {
		TypedValue value = getTypedValueForVariable(variableName, deserializeValue);
		return VariableValueDto.fromTypedValue(value);

	  }

	  protected internal virtual TypedValue getTypedValueForVariable(string variableName, bool deserializeValue)
	  {
		TypedValue value = null;
		try
		{
		   value = getVariableEntity(variableName, deserializeValue);
		}
		catch (AuthorizationException e)
		{
		  throw e;
		}
		catch (ProcessEngineException e)
		{
		  string errorMessage = string.Format("Cannot get {0} variable {1}: {2}", ResourceTypeName, variableName, e.Message);
		  throw new RestException(Response.Status.INTERNAL_SERVER_ERROR, e, errorMessage);
		}

		if (value == null)
		{
		  string errorMessage = string.Format("{0} variable with name {1} does not exist", ResourceTypeName, variableName);
		  throw new InvalidRequestException(Response.Status.NOT_FOUND, errorMessage);
		}
		return value;
	  }

	  public virtual Response getVariableBinary(string variableName)
	  {
		TypedValue typedValue = getTypedValueForVariable(variableName, false);
		return (new VariableResponseProvider()).getResponseForTypedVariable(typedValue, resourceId);
	  }

	  public virtual void putVariable(string variableName, VariableValueDto variable)
	  {

		try
		{
		  TypedValue typedValue = variable.toTypedValue(engine, objectMapper);
		  setVariableEntity(variableName, typedValue);

		}
		catch (RestException e)
		{
		  throw new InvalidRequestException(e.Status, e, string.Format("Cannot put {0} variable {1}: {2}", ResourceTypeName, variableName, e.Message));
		}
		catch (BadUserRequestException e)
		{
		  throw new RestException(Response.Status.BAD_REQUEST, e, string.Format("Cannot put {0} variable {1}: {2}", ResourceTypeName, variableName, e.Message));
		}
		catch (AuthorizationException e)
		{
		  throw e;
		}
		catch (ProcessEngineException e)
		{
		  throw new RestException(Response.Status.INTERNAL_SERVER_ERROR, e, string.Format("Cannot put {0} variable {1}: {2}", ResourceTypeName, variableName, e.Message));
		}
	  }

	  public virtual void setBinaryVariable(string variableKey, MultipartFormData payload)
	  {
		MultipartFormData.FormPart dataPart = payload.getNamedPart("data");
		MultipartFormData.FormPart objectTypePart = payload.getNamedPart("type");
		MultipartFormData.FormPart valueTypePart = payload.getNamedPart("valueType");

		if (objectTypePart != null)
		{
		  object @object = null;

		  if (!string.ReferenceEquals(dataPart.ContentType, null) && dataPart.ContentType.ToLower().Contains(MediaType.APPLICATION_JSON))
		  {

			@object = deserializeJsonObject(objectTypePart.TextContent, dataPart.BinaryContent);

		  }
		  else
		  {
			throw new InvalidRequestException(Response.Status.BAD_REQUEST, "Unrecognized content type for serialized java type: " + dataPart.ContentType);
		  }

		  if (@object != null)
		  {
			setVariableEntity(variableKey, Variables.objectValue(@object).create());
		  }
		}
		else
		{

		  string valueTypeName = DEFAULT_BINARY_VALUE_TYPE;
		  if (valueTypePart != null)
		  {
			if (string.ReferenceEquals(valueTypePart.TextContent, null))
			{
			  throw new InvalidRequestException(Response.Status.BAD_REQUEST, "Form part with name 'valueType' must have a text/plain value");
			}

			valueTypeName = valueTypePart.TextContent;
		  }

		  VariableValueDto valueDto = VariableValueDto.fromFormPart(valueTypeName, dataPart);
		  try
		  {

			TypedValue typedValue = valueDto.toTypedValue(engine, objectMapper);
			setVariableEntity(variableKey, typedValue);
		  }
		  catch (AuthorizationException e)
		  {
			throw e;
		  }
		  catch (ProcessEngineException e)
		  {
			string errorMessage = string.Format("Cannot put {0} variable {1}: {2}", ResourceTypeName, variableKey, e.Message);
			throw new RestException(Response.Status.INTERNAL_SERVER_ERROR, e, errorMessage);
		  }
		}
	  }

	  protected internal virtual object deserializeJsonObject(string className, sbyte[] data)
	  {
		try
		{
		  JavaType type = TypeFactory.defaultInstance().constructFromCanonical(className);

		  return objectMapper.readValue(StringHelper.NewString(data, Charset.forName("UTF-8")), type);

		}
		catch (Exception e)
		{
		  throw new InvalidRequestException(Response.Status.INTERNAL_SERVER_ERROR, "Could not deserialize JSON object: " + e.Message);

		}
	  }

	  public virtual void deleteVariable(string variableName)
	  {
		try
		{
		  removeVariableEntity(variableName);
		}
		catch (AuthorizationException e)
		{
		  throw e;
		}
		catch (ProcessEngineException e)
		{
		  string errorMessage = string.Format("Cannot delete {0} variable {1}: {2}", ResourceTypeName, variableName, e.Message);
		  throw new RestException(Response.Status.INTERNAL_SERVER_ERROR, e, errorMessage);
		}

	  }

	  public virtual void modifyVariables(PatchVariablesDto patch)
	  {
		VariableMap variableModifications = null;
		try
		{
		  variableModifications = VariableValueDto.toMap(patch.Modifications, engine, objectMapper);

		}
		catch (RestException e)
		{
		  string errorMessage = string.Format("Cannot modify variables for {0}: {1}", ResourceTypeName, e.Message);
		  throw new InvalidRequestException(e.Status, e, errorMessage);

		}

		IList<string> variableDeletions = patch.Deletions;

		try
		{
		  updateVariableEntities(variableModifications, variableDeletions);
		}
		catch (AuthorizationException e)
		{
		  throw e;
		}
		catch (ProcessEngineException e)
		{
		  string errorMessage = string.Format("Cannot modify variables for {0} {1}: {2}", ResourceTypeName, resourceId, e.Message);
		  throw new RestException(Response.Status.INTERNAL_SERVER_ERROR, e, errorMessage);
		}


	  }

	  protected internal abstract VariableMap getVariableEntities(bool deserializeValues);

	  protected internal abstract void updateVariableEntities(VariableMap variables, IList<string> deletions);

	  protected internal abstract TypedValue getVariableEntity(string variableKey, bool deserializeValue);

	  protected internal abstract void setVariableEntity(string variableKey, TypedValue variableValue);

	  protected internal abstract void removeVariableEntity(string variableKey);

	  protected internal abstract string ResourceTypeName {get;}

	}

}