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
namespace org.camunda.spin.plugin.variable.value
{
	using SerializableValue = org.camunda.bpm.engine.variable.value.SerializableValue;
	using DataFormat = org.camunda.spin.spi.DataFormat;

	/// <summary>
	/// @author Roman Smirnov
	/// 
	/// </summary>
	public interface SpinValue : SerializableValue
	{

//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: public org.camunda.spin.Spin<?> getValue();
	  Spin<object> Value {get;}

//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: public org.camunda.spin.spi.DataFormat<? extends org.camunda.spin.Spin<?>> getDataFormat();
	  DataFormat<Spin<object>> DataFormat {get;}

	}

}