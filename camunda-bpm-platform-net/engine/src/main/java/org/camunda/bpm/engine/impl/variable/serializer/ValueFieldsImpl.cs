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
namespace org.camunda.bpm.engine.impl.variable.serializer
{
	/// <summary>
	/// @author Thorben Lindhauer
	/// 
	/// </summary>
	public class ValueFieldsImpl : ValueFields
	{

	  protected internal string text;
	  protected internal string text2;
	  protected internal long? longValue;
	  protected internal double? doubleValue;
	  protected internal sbyte[] byteArrayValue;

	  public virtual string Name
	  {
		  get
		  {
			return null;
		  }
	  }

	  public virtual string TextValue
	  {
		  get
		  {
			return text;
		  }
		  set
		  {
			this.text = value;
		  }
	  }


	  public virtual string TextValue2
	  {
		  get
		  {
			return text2;
		  }
		  set
		  {
			this.text2 = value;
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


	  public virtual sbyte[] ByteArrayValue
	  {
		  get
		  {
			return byteArrayValue;
		  }
		  set
		  {
			this.byteArrayValue = value;
		  }
	  }


	}

}