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
namespace org.camunda.bpm.integrationtest.jobexecutor.beans
{


	[Serializable]
	public class DemoVariableClass
	{

	  private const long serialVersionUID = 1L;
	  private bool booleanProperty;
	  private sbyte byteProperty;
	  private short shortProperty;
	  private int intProperty;
	  private long longProperty;
	  private float floatProperty;
	  private double doubleProperty;
	  private char charProperty;

	  private bool? booleanObjectProperty;
	  private sbyte? byteObjectProperty;
	  private short? shortObjectProperty;
	  private int? integerObjectProperty;
	  private long? longObjectProperty;
	  private float? floatObjectProperty;
	  private double? doubleObjectProperty;

	  private string stringProperty;
	  private int[] intArrayProperty;
	  private IDictionary<object, object> mapProperty;

	  public virtual bool BooleanProperty
	  {
		  get
		  {
			return booleanProperty;
		  }
		  set
		  {
			this.booleanProperty = value;
		  }
	  }


	  public virtual sbyte ByteProperty
	  {
		  get
		  {
			return byteProperty;
		  }
		  set
		  {
			this.byteProperty = value;
		  }
	  }


	  public virtual short ShortProperty
	  {
		  get
		  {
			return shortProperty;
		  }
		  set
		  {
			this.shortProperty = value;
		  }
	  }


	  public virtual int IntProperty
	  {
		  get
		  {
			return intProperty;
		  }
		  set
		  {
			this.intProperty = value;
		  }
	  }


	  public virtual long LongProperty
	  {
		  get
		  {
			return longProperty;
		  }
		  set
		  {
			this.longProperty = value;
		  }
	  }


	  public virtual float FloatProperty
	  {
		  get
		  {
			return floatProperty;
		  }
		  set
		  {
			this.floatProperty = value;
		  }
	  }


	  public virtual double DoubleProperty
	  {
		  get
		  {
			return doubleProperty;
		  }
		  set
		  {
			this.doubleProperty = value;
		  }
	  }


	  public virtual char CharProperty
	  {
		  get
		  {
			return charProperty;
		  }
		  set
		  {
			this.charProperty = value;
		  }
	  }


	  public virtual bool? BooleanObjectProperty
	  {
		  get
		  {
			return booleanObjectProperty;
		  }
		  set
		  {
			this.booleanObjectProperty = value;
		  }
	  }


	  public virtual sbyte? ByteObjectProperty
	  {
		  get
		  {
			return byteObjectProperty;
		  }
		  set
		  {
			this.byteObjectProperty = value;
		  }
	  }


	  public virtual short? ShortObjectProperty
	  {
		  get
		  {
			return shortObjectProperty;
		  }
		  set
		  {
			this.shortObjectProperty = value;
		  }
	  }


	  public virtual int? IntegerObjectProperty
	  {
		  get
		  {
			return integerObjectProperty;
		  }
		  set
		  {
			this.integerObjectProperty = value;
		  }
	  }


	  public virtual long? LongObjectProperty
	  {
		  get
		  {
			return longObjectProperty;
		  }
		  set
		  {
			this.longObjectProperty = value;
		  }
	  }


	  public virtual float? FloatObjectProperty
	  {
		  get
		  {
			return floatObjectProperty;
		  }
		  set
		  {
			this.floatObjectProperty = value;
		  }
	  }


	  public virtual double? DoubleObjectProperty
	  {
		  get
		  {
			return doubleObjectProperty;
		  }
		  set
		  {
			this.doubleObjectProperty = value;
		  }
	  }


	  public virtual string StringProperty
	  {
		  get
		  {
			return stringProperty;
		  }
		  set
		  {
			this.stringProperty = value;
		  }
	  }


	  public virtual int[] IntArrayProperty
	  {
		  get
		  {
			return intArrayProperty;
		  }
		  set
		  {
			this.intArrayProperty = value;
		  }
	  }


	  public virtual IDictionary<object, object> MapProperty
	  {
		  get
		  {
			return mapProperty;
		  }
		  set
		  {
			this.mapProperty = value;
		  }
	  }


	}
}