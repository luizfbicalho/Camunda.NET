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
namespace org.camunda.bpm.integrationtest.functional.spin.dataformat
{

	using DataFormat = org.camunda.spin.spi.DataFormat;
	using DataFormatMapper = org.camunda.spin.spi.DataFormatMapper;
	using DataFormatReader = org.camunda.spin.spi.DataFormatReader;
	using DataFormatWriter = org.camunda.spin.spi.DataFormatWriter;
	using TextBasedDataFormatReader = org.camunda.spin.spi.TextBasedDataFormatReader;

	/// <summary>
	/// @author Thorben Lindhauer
	/// 
	/// </summary>
	public class FooDataFormat : DataFormat<FooSpin>
	{

	  public const string TYPE_NAME = "FooType";
	  public const string NAME = "application/foo";

	  public virtual string Name
	  {
		  get
		  {
			return NAME;
		  }
	  }

	  public virtual Type WrapperType
	  {
		  get
		  {
			return typeof(FooSpin);
		  }
	  }

	  public virtual FooSpin createWrapperInstance(object parameter)
	  {
		return new FooSpin();
	  }

	  public virtual DataFormatReader Reader
	  {
		  get
		  {
			return new TextBasedDataFormatReaderAnonymousInnerClass(this);
		  }
	  }

	  private class TextBasedDataFormatReaderAnonymousInnerClass : TextBasedDataFormatReader
	  {
		  private readonly FooDataFormat outerInstance;

		  public TextBasedDataFormatReaderAnonymousInnerClass(FooDataFormat outerInstance)
		  {
			  this.outerInstance = outerInstance;
		  }


		  public override object readInput(Reader reader)
		  {
			return null;
		  }

		  protected internal override Pattern InputDetectionPattern
		  {
			  get
			  {
				return Pattern.compile("foo");
			  }
		  }

	  }

	  public override DataFormatWriter Writer
	  {
		  get
		  {
			return new DataFormatWriterAnonymousInnerClass(this);
		  }
	  }

	  private class DataFormatWriterAnonymousInnerClass : DataFormatWriter
	  {
		  private readonly FooDataFormat outerInstance;

		  public DataFormatWriterAnonymousInnerClass(FooDataFormat outerInstance)
		  {
			  this.outerInstance = outerInstance;
		  }


		  public override void writeToWriter(Writer writer, object input)
		  {
			try
			{
			  writer.write("foo");
			}
			catch (IOException e)
			{
			  throw new Exception(e);
			}
		  }
	  }

	  public override DataFormatMapper Mapper
	  {
		  get
		  {
			return new DataFormatMapperAnonymousInnerClass(this);
		  }
	  }

	  private class DataFormatMapperAnonymousInnerClass : DataFormatMapper
	  {
		  private readonly FooDataFormat outerInstance;

		  public DataFormatMapperAnonymousInnerClass(FooDataFormat outerInstance)
		  {
			  this.outerInstance = outerInstance;
		  }


		  public override object mapJavaToInternal(object parameter)
		  {
			return null;
		  }

		  public override T mapInternalToJava<T>(object parameter, string typeIdentifier)
		  {
			return (T) new Foo();
		  }

		  public override T mapInternalToJava<T>(object parameter, Type type)
		  {
				  type = typeof(T);
			return null;
		  }

		  public override string getCanonicalTypeName(object @object)
		  {
			return TYPE_NAME;
		  }

		  public override bool canMap(object parameter)
		  {
			return parameter is Foo;
		  }
	  }

	}

}