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
namespace org.camunda.bpm.engine.impl.history.@event
{


	/// <summary>
	/// @author Daniel Meyer
	/// 
	/// </summary>
	[Serializable]
	public class HistoricVariableUpdateEventEntity : HistoricDetailEventEntity
	{

	  private const long serialVersionUID = 1L;

	  protected internal int revision;

	  protected internal string variableName;
	  protected internal string variableInstanceId;
	  protected internal string scopeActivityInstanceId;

	  protected internal string serializerName;

	  protected internal long? longValue;
	  protected internal double? doubleValue;
	  protected internal string textValue;
	  protected internal string textValue2;
	  protected internal sbyte[] byteValue;

	  protected internal string byteArrayId;

	  // getter / setters ////////////////////////////

	  public virtual string SerializerName
	  {
		  get
		  {
			return serializerName;
		  }
		  set
		  {
			this.serializerName = value;
		  }
	  }
	  public virtual string VariableName
	  {
		  get
		  {
			return variableName;
		  }
		  set
		  {
			this.variableName = value;
		  }
	  }
	  public virtual long? LongValue
	  {
		  get
		  {
			return longValue;
		  }
		  set
		  {
			this.longValue = value;
		  }
	  }
	  public virtual double? DoubleValue
	  {
		  get
		  {
			return doubleValue;
		  }
		  set
		  {
			this.doubleValue = value;
		  }
	  }
	  public virtual string TextValue
	  {
		  get
		  {
			return textValue;
		  }
		  set
		  {
			this.textValue = value;
		  }
	  }
	  public virtual string TextValue2
	  {
		  get
		  {
			return textValue2;
		  }
		  set
		  {
			this.textValue2 = value;
		  }
	  }
	  public virtual sbyte[] ByteValue
	  {
		  get
		  {
			return byteValue;
		  }
		  set
		  {
			this.byteValue = value;
		  }
	  }
	  public virtual int Revision
	  {
		  get
		  {
			return revision;
		  }
		  set
		  {
			this.revision = value;
		  }
	  }
	  public virtual string ByteArrayId
	  {
		  set
		  {
			byteArrayId = value;
		  }
		  get
		  {
			return byteArrayId;
		  }
	  }
	  public virtual string VariableInstanceId
	  {
		  get
		  {
			return variableInstanceId;
		  }
		  set
		  {
			this.variableInstanceId = value;
		  }
	  }
	  public virtual string ScopeActivityInstanceId
	  {
		  get
		  {
			return scopeActivityInstanceId;
		  }
		  set
		  {
			this.scopeActivityInstanceId = value;
		  }
	  }

	  public override string ToString()
	  {
		return this.GetType().Name + "[variableName=" + variableName + ", variableInstanceId=" + variableInstanceId + ", revision=" + revision + ", serializerName=" + serializerName + ", longValue=" + longValue + ", doubleValue=" + doubleValue + ", textValue=" + textValue + ", textValue2=" + textValue2 + ", byteArrayId=" + byteArrayId + ", activityInstanceId=" + activityInstanceId + ", scopeActivityInstanceId=" + scopeActivityInstanceId + ", eventType=" + eventType + ", executionId=" + executionId + ", id=" + id + ", processDefinitionId=" + processInstanceId + ", processInstanceId=" + processInstanceId + ", taskId=" + taskId + ", timestamp=" + timestamp + ", tenantId=" + tenantId + "]";
	  }

	}

}